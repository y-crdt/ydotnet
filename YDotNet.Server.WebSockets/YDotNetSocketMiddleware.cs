using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace YDotNet.Server.WebSockets;

public sealed class YDotNetSocketMiddleware : IDocumentCallback
{
    private readonly ConcurrentDictionary<string, List<ClientState>> statesPerDocumentName = new();
    private readonly YDotNetWebSocketOptions options;
    private readonly ILogger<YDotNetSocketMiddleware> logger;
    private IDocumentManager? documentManager;

    public YDotNetSocketMiddleware(IOptions<YDotNetWebSocketOptions> options, ILogger<YDotNetSocketMiddleware> logger)
    {
        this.options = options.Value;
        this.logger = logger;
    }

    public ValueTask OnInitializedAsync(IDocumentManager manager)
    {
        documentManager = manager;
        return default;
    }

    public ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        Task.Run(async () =>
        {
            var documentStates = GetOtherClients(@event.Context.DocumentName, @event.Context.ClientId);

            foreach (var state in documentStates)
            {
                await state.WriteLockedAsync(@event, async (encoder, @event, _, ct) =>
                {
                    await encoder.WriteVarUintAsync(MessageTypes.TypeAwareness, ct);
                    await encoder.WriteVarUintAsync(1, ct);
                    await encoder.WriteAwarenessAsync(@event.Context.ClientId, @event.ClientClock, @event.ClientState, ct);
                }, default);
            }
        });

        return default;
    }

    public ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        Task.Run(async () =>
        {
            var documentStates = GetOtherClients(@event.Context.DocumentName, @event.Context.ClientId);

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

        return default;
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

        logger.LogDebug("Websocket connection to {document} established.", documentName);

        var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

        using var state = new ClientState
        {
            Decoder = new WebSocketDecoder(webSocket),
            DocumentContext = new DocumentContext(documentName, 0),
            DocumentName = documentName,
            Encoder = new WebSocketEncoder(webSocket),
            WebSocket = webSocket
        };

        // Usually we should have relatively few clients per document, therefore we use a simple lock.
        lock (documentStates)
        {
            documentStates.Add(state);
        }

        await AuthenticateAsync(httpContext, state);

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
        catch (OperationCanceledException)
        {
            // Usually throw when the client stops the connection.
        }
        catch (WebSocketException)
        {
            // Usually throw when the client stops the connection.
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

        logger.LogDebug("Websocket connection to {document} closed.", documentName);
    }

    private async Task AuthenticateAsync(HttpContext httpContext, ClientState state)
    {
        if (options.OnAuthenticateAsync == null)
        {
            return;
        }

        try
        {
            await options.OnAuthenticateAsync(httpContext, state.DocumentContext);
        }
        catch (Exception ex)
        {
            await state.Encoder.WriteAuthErrorAsync(ex.Message, httpContext.RequestAborted);
            throw;
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
                    var clientState = await state.Decoder.ReadVarUint8ArrayAsync(ct);

                    var serverState = await documentManager!.GetStateAsync(state.DocumentContext, ct);
                    var serverUpdate = await documentManager!.GetStateAsUpdateAsync(state.DocumentContext, clientState, ct);

                    // We mark the sync state as false again to handle multiple sync steps.
                    state.IsSynced = false;

                    await encoder.WriteSyncStep2Async(serverUpdate, ct);
                    await encoder.WriteSyncStep1Async(serverState, ct);

                    await SendPendingUpdatesAsync(encoder, state, ct);
                    await SendAwarenessAsync(encoder, state, ct);

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

    private static async Task SendPendingUpdatesAsync(WebSocketEncoder encoder, ClientState state, CancellationToken ct)
    {
        while (state.PendingUpdates.TryDequeue(out var pendingDiff))
        {
            await encoder.WriteSyncUpdateAsync(pendingDiff, ct);
        }
    }

    private async Task SendAwarenessAsync(WebSocketEncoder encoder, ClientState state, CancellationToken ct)
    {
        var users = await documentManager!.GetAwarenessAsync(state.DocumentContext, ct);

        if (users.Count == 0)
        {
            return;
        }

        await encoder.WriteVarUintAsync(MessageTypes.TypeAwareness, ct);
        await encoder.WriteVarUintAsync(users.Count, ct);

        foreach (var (clientId, user) in users)
        {
            await encoder.WriteAwarenessAsync(clientId, user.ClientClock, user.ClientState, ct);
        }
    }

    private async Task HandleAwarenessAsync(ClientState state,
        CancellationToken ct)
    {
        // This is the length of the awareness message (for whatever reason).
        await state.Decoder.ReadVarUintAsync(ct);

        var clientCount = await state.Decoder.ReadVarUintAsync(ct);

        for (var i = 0; i < clientCount; i++)
        {
            var clientId = await state.Decoder.ReadVarUintAsync(ct);
            var clientClock = await state.Decoder.ReadVarUintAsync(ct);
            var clientState = await state.Decoder.ReadVarStringAsync(ct);

            if (state.DocumentContext.ClientId == 0 && clientCount == 1)
            {
                state.DocumentContext = state.DocumentContext with { ClientId = clientId };

                logger.LogDebug("Websocket connection to {document} enhanced with client Id {clientId}.",
                    state.DocumentContext.DocumentName,
                    state.DocumentContext.ClientId);
            }

            var context = new DocumentContext(state.DocumentName, clientId);

            await documentManager!.PingAsync(context, clientClock, clientState, ct);
        }
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
