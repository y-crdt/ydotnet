using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace YDotNet.Server.WebSockets;

public sealed class YDotNetSocketMiddleware : IDocumentCallback
{
    private readonly ConcurrentDictionary<string, List<ClientState>> statesPerDocumentName = new ConcurrentDictionary<string, List<ClientState>>();
    private IDocumentManager? documentManager;

    public ValueTask OnInitializedAsync(IDocumentManager manager)
    {
        documentManager = manager;
        return default;
    }

    public async ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        await Task.Run(async () =>
        {
            var documentStates = GetOtherClients(@event.DocumentContext.DocumentName, @event.DocumentContext.ClientId);

            foreach (var state in documentStates)
            {
                await state.WriteLockedAsync(@event, async (encoder, @event, _, ct) =>
                {
                    await encoder.WriteVarUintAsync(MessageTypes.TypeAwareness, ct);
                    await encoder.WriteVarUintAsync(1, ct);
                    await encoder.WriteAwarenessAsync(@event.DocumentContext.ClientId, @event.ClientClock, @event.ClientState, ct);
                }, default);
            }
        });
    }

    public async ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        await Task.Run(async () =>
        {
            var documentStates = GetOtherClients(@event.DocumentContext.DocumentName, @event.DocumentContext.ClientId);

            foreach (var state in documentStates)
            {
                await state.WriteLockedAsync(@event.Diff, async (encoder, diff, _, ct) =>
                {
                    if (state.IsSynced)
                    {
                        await encoder.WriteSyncUpdateAsync(diff, ct);
                    }
                    else
                    {
                        state.PendingUpdates.Enqueue(@event.Diff);
                    }
                }, default);
            }
        });
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (!httpContext.WebSockets.IsWebSocketRequest || documentManager == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var documentName = httpContext.Request.Path;
        var documentStates = statesPerDocumentName.GetOrAdd(documentName, _ => new List<ClientState>());

        var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

        using var state = new ClientState
        {
            Decoder = new WebSocketDecoder(webSocket),
            DocumentContext = new DocumentContext { ClientId = 0, DocumentName = documentName },
            DocumentName = documentName,
            Encoder = new WebSocketEncoder(webSocket),
            WebSocket = webSocket
        };

        // Usually we should have relatively few clients per document, therefore we use a simple lock.
        lock (documentStates)
        {
            documentStates.Add(state);
        }

        try
        {
            while (state.Decoder.CanRead)
            {
                var messageType = await state.Decoder.ReadVarUintAsync(httpContext.RequestAborted);

                switch (messageType)
                {
                    case MessageTypes.TypeSync:
                        await HandleSyncAsync(state, httpContext.RequestAborted);
                        break;

                    case MessageTypes.TypeAwareness:
                        await HandleAwarenessAsync(state, httpContext.RequestAborted);
                        break;

                    default:
                        throw new InvalidOperationException("Protocol error.");
                }
            }
        }
        finally
        {
            // Usually we should have relatively few clients per document, therefore we use a simple lock.
            lock (documentStates)
            {
                documentStates.Remove(state);
            }

            if (state.DocumentContext.ClientId != 0)
            {
                await documentManager.DisconnectAsync(state.DocumentContext, default);
            }
        }
    }

    private async Task HandleSyncAsync(ClientState state,
        CancellationToken ct)
    {
        await state.WriteLockedAsync(true, async (encoder, context, state, ct) =>
        {
            if (!state.Decoder.HasMore)
            {
                return;
            }

            var syncType = await state.Decoder.ReadVarUintAsync(ct);

            switch (syncType)
            {
                case MessageTypes.SyncStep1:
                    var stateVector = await state.Decoder.ReadVarUint8ArrayAsync(ct);

                    var (update, serverState) = await documentManager!.GetMissingChangesAsync(state.DocumentContext, stateVector, ct);

                    // We mark the sync state as false again to handle multiple sync steps.
                    state.IsSynced = false;

                    await encoder.WriteSyncStep2Async(update, ct);
                    await encoder.WriteSyncStep1Async(serverState, ct);

                    while (state.PendingUpdates.TryDequeue(out var pendingDiff))
                    {
                        await encoder.WriteSyncUpdateAsync(pendingDiff, ct);
                    }

                    var users = await documentManager.GetAwarenessAsync(state.DocumentName, ct);

                    if (users.Count != 0)
                    {
                        await encoder.WriteVarUintAsync(MessageTypes.TypeAwareness, ct);
                        await encoder.WriteVarUintAsync(users.Count, ct);

                        foreach (var (clientId, user) in await documentManager.GetAwarenessAsync(state.DocumentName, ct))
                        {
                            await encoder.WriteAwarenessAsync(clientId, user.ClientClock, user.ClientState, ct);
                        }
                    }

                    // Sync state has been completed, therefore the client will receive normal updates now.
                    state.IsSynced = true;
                    break;

                case MessageTypes.SyncStep2:
                case MessageTypes.SyncUpdate:
                    var diff = await state.Decoder.ReadVarUint8ArrayAsync(ct);
                    
                    await documentManager!.ApplyUpdateAsync(state.DocumentContext, diff, ct);
                    break;

                default:
                    throw new InvalidOperationException("Protocol error.");
            }
        }, ct);
    }

    private async Task HandleAwarenessAsync(ClientState state,
        CancellationToken ct)
    {
        // This is the length of the awareness message (for whatever reason).
        await state.Decoder.ReadVarUintAsync(ct);

        var clientCount = await state.Decoder.ReadVarUintAsync(ct);

        if (clientCount != 1)
        {
            throw new InvalidOperationException($"Protocol error. Expected client count to be 1, got {clientCount}.");
        }

        var clientId = await state.Decoder.ReadVarUintAsync(ct);
        var clientClock = await state.Decoder.ReadVarUintAsync(ct);
        var clientState = await state.Decoder.ReadVarStringAsync(ct);
        
        state.DocumentContext.ClientId = clientId;

        await documentManager!.PingAsync(state.DocumentContext, clientClock, clientState, ct);
    }

    private List<ClientState> GetOtherClients(string documentName, long clientId)
    {
        var documentStates = statesPerDocumentName.GetOrAdd(documentName, _ => new List<ClientState>());

        // Usually we should have relatively few clients per document, therefore we use a simple lock.
        lock (documentStates)
        {
            return documentStates.Where(x => x.DocumentContext.ClientId != clientId).ToList();
        }
    }
}
