using System.Collections.Concurrent;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YDotNet.Protocol;

#pragma warning disable SA1117 // Parameters should be on same line or separate lines

namespace YDotNet.Server.WebSockets;

public sealed class YDotNetSocketMiddleware : IDocumentCallback
{
    private readonly ConcurrentDictionary<string, List<ClientState>> statesPerDocumentName = new(StringComparer.Ordinal);
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
        _ = Task.Run(async () =>
        {
            var documentStates = GetOtherClients(@event.Context.DocumentName, @event.Context.ClientId);

            foreach (var state in documentStates)
            {
                await state.WriteLockedAsync(@event, async (encoder, e, _, ct) =>
                {
                    var message = new AwarenessMessage(new AwarenessInformation(e.Context.ClientId, e.ClientClock, (string?)e.ClientState));

                    await encoder.WriteAsync(message, ct).ConfigureAwait(false);
                }, default).ConfigureAwait(false);
            }
        });

        return default;
    }

    public ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        _ = Task.Run(async () =>
        {
            var documentStates = GetOtherClients(@event.Context.DocumentName, @event.Context.ClientId);

            foreach (var state in documentStates)
            {
                await state.WriteLockedAsync(@event.Diff, async (encoder, diff, _, ct) =>
                {
                    if (state.IsSynced)
                    {
                        var message = new SyncUpdateMessage(diff);

                        await encoder.WriteAsync(message, ct).ConfigureAwait(false);
                    }
                    else
                    {
                        state.PendingUpdates.Enqueue(@event.Diff);
                    }
                }, default).ConfigureAwait(false);
            }
        });

        return default;
    }

    public Task InvokeAsync(HttpContext httpContext)
    {
        var documentName = httpContext.Request.Path.ToString()[1..];

        return InvokeAsync(httpContext, documentName);
    }

    public async Task InvokeAsync(HttpContext httpContext, string documentName)
    {
        if (!httpContext.WebSockets.IsWebSocketRequest || documentManager == null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return;
        }

        var state = await EstablishConnectionAsync(httpContext, documentName).ConfigureAwait(false);
        try
        {
            while (state.Decoder.CanRead)
            {
                var message = await state.Decoder.ReadNextMessageAsync(httpContext.RequestAborted).ConfigureAwait(false);

                switch (message)
                {
                    case SyncStep1Message sync1:
                        await HandleAsync(state, sync1, httpContext.RequestAborted).ConfigureAwait(false);
                        break;

                    case SyncStep2Message sync2:
                        await HandleAsync(state, sync2, httpContext.RequestAborted).ConfigureAwait(false);
                        break;

                    case SyncUpdateMessage syncUpdate:
                        await HandleAsync(state, syncUpdate, httpContext.RequestAborted).ConfigureAwait(false);
                        break;

                    case AwarenessMessage awareness:
                        await HandleAwarenessAsync(state, awareness, httpContext.RequestAborted).ConfigureAwait(false);
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
            await CleanupAsync(state).ConfigureAwait(false);
        }

        logger.LogDebug("Websocket connection to {document} closed.", documentName);
    }

    private async Task<ClientState> EstablishConnectionAsync(HttpContext httpContext, string documentName)
    {
        var documentStates = statesPerDocumentName.GetOrAdd(documentName, _ => new List<ClientState>());

        logger.LogDebug("Websocket connection to {document} established.", documentName);

        var webSocket = await httpContext.WebSockets.AcceptWebSocketAsync().ConfigureAwait(false);

        var state = new ClientState
        {
            Decoder = new WebSocketDecoder(webSocket),
            DocumentContext = new DocumentContext(documentName, 0),
            DocumentName = documentName,
            Encoder = new WebSocketEncoder(webSocket),
            WebSocket = webSocket,
        };

        // Usually we should have relatively few clients per document, therefore we use a simple lock.
        lock (documentStates)
        {
            documentStates.Add(state);
        }

        await AuthenticateAsync(httpContext, state).ConfigureAwait(false);
        return state;
    }

    private async Task CleanupAsync(ClientState state)
    {
        var documentStates = statesPerDocumentName.GetOrAdd(state.DocumentName, _ => new List<ClientState>());

        // Usually we should have relatively few clients per document, therefore we use a simple lock.
        lock (documentStates)
        {
            documentStates.Remove(state);
        }

        if (state.DocumentContext.ClientId != 0)
        {
            await documentManager!.DisconnectAsync(state.DocumentContext, default).ConfigureAwait(false);
        }
    }

    private async Task AuthenticateAsync(HttpContext httpContext, ClientState state)
    {
        if (options.OnAuthenticateAsync == null)
        {
            return;
        }

        try
        {
            await options.OnAuthenticateAsync(httpContext, state.DocumentContext).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var message = new AuthErrorMessage(ex.Message);

            await state.Encoder.WriteAsync(message, httpContext.RequestAborted).ConfigureAwait(false);
            throw;
        }
    }

    private async Task HandleAsync(ClientState state, SyncStep1Message message, CancellationToken ct)
    {
        await state.WriteLockedAsync(state: message, async (encoder, message, state, ct) =>
        {
            var serverState = await documentManager!.GetStateVectorAsync(state.DocumentContext, ct).ConfigureAwait(false);
            var serverUpdate = await documentManager!.GetUpdateAsync(state.DocumentContext, message.StateVector, ct).ConfigureAwait(false);

            // We mark the sync state as false again to handle multiple sync steps.
            state.IsSynced = false;

            await encoder.WriteAsync(new SyncStep2Message(serverUpdate), ct).ConfigureAwait(false);
            await encoder.WriteAsync(new SyncStep1Message(serverState), ct).ConfigureAwait(false);

            await SendPendingUpdatesAsync(encoder, state, ct).ConfigureAwait(false);
            await SendAwarenessAsync(encoder, state, ct).ConfigureAwait(false);

            // Sync state has been completed, therefore the client will receive normal updates now.
            state.IsSynced = true;
        }, ct).ConfigureAwait(false);
    }

    private async Task HandleAsync(ClientState state, SyncStep2Message message, CancellationToken ct)
    {
        await state.WriteLockedAsync(state: message, async (encoder, message, state, ct) =>
        {
            await documentManager!.ApplyUpdateAsync(state.DocumentContext, message.Update, ct).ConfigureAwait(false);
        }, ct).ConfigureAwait(false);
    }

    private async Task HandleAsync(ClientState state, SyncUpdateMessage message, CancellationToken ct)
    {
        await state.WriteLockedAsync(state: message, async (encoder, message, state, ct) =>
        {
            await documentManager!.ApplyUpdateAsync(state.DocumentContext, message.Update, ct).ConfigureAwait(false);
        }, ct).ConfigureAwait(false);
    }

    private async Task HandleAwarenessAsync(ClientState state, AwarenessMessage awareness, CancellationToken ct)
    {
        if (state.DocumentContext.ClientId == 0 && awareness.Clients.Length == 1)
        {
            var (clientId, _, _) = awareness.Clients[0];

            state.DocumentContext = state.DocumentContext with { ClientId = clientId };

            logger.LogDebug(
                "Websocket connection to {document} enhanced with client Id {clientId}.",
                state.DocumentContext.DocumentName,
                state.DocumentContext.ClientId);
        }

        foreach (var (clientId, clock, clientState) in awareness.Clients)
        {
            var context = new DocumentContext(state.DocumentName, clientId);

            await documentManager!.PingAsync(context, clock, clientState, ct).ConfigureAwait(false);
        }
    }

    private static async Task SendPendingUpdatesAsync(WebSocketEncoder encoder, ClientState state, CancellationToken ct)
    {
        while (state.PendingUpdates.TryDequeue(out var pendingDiff))
        {
            var message = new SyncUpdateMessage(pendingDiff);

            await encoder.WriteAsync(message, ct).ConfigureAwait(false);
        }
    }

    private async Task SendAwarenessAsync(WebSocketEncoder encoder, ClientState state, CancellationToken ct)
    {
        var users = await documentManager!.GetAwarenessAsync(state.DocumentContext, ct).ConfigureAwait(false);

        var message =
            new AwarenessMessage(
                users.Select(client => new AwarenessInformation(
                    client.Key,
                    client.Value.ClientClock,
                    client.Value.ClientState)).ToArray());

        await encoder.WriteAsync(message, ct).ConfigureAwait(false);
    }

    private List<ClientState> GetOtherClients(string documentName, ulong clientId)
    {
        var documentStates = statesPerDocumentName.GetOrAdd(documentName, _ => new List<ClientState>());

        // Usually we should have relatively few clients per document, therefore we use a simple lock.
        lock (documentStates)
        {
            return documentStates.Where(x => x.DocumentContext.ClientId != clientId).ToList();
        }
    }
}
