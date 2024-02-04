using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YDotNet.Document;
using YDotNet.Server.Internal;
using YDotNet.Server.Storage;
using IDocumentCallbacks = System.Collections.Generic.IEnumerable<YDotNet.Server.IDocumentCallback>;

#pragma warning disable IDE0063 // Use simple 'using' statement

namespace YDotNet.Server;

public sealed class DefaultDocumentManager : IDocumentManager
{
    private readonly ConnectedUsers users = new();
    private readonly DocumentManagerOptions options;
    private readonly DocumentCache cache;
    private readonly CallbackInvoker callback;

    public DefaultDocumentManager(
        IDocumentStorage documentStorage,
        IDocumentCallbacks callbacks,
        IOptions<DocumentManagerOptions> options,
        ILogger<DefaultDocumentManager> logger)
    {
        this.options = options.Value;
        callback = new CallbackInvoker(callbacks, logger);

        cache = new DocumentCache(documentStorage, callback, this, options.Value);
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        await callback.OnInitializedAsync(this).ConfigureAwait(false);
    }

    public async Task StopAsync(
        CancellationToken cancellationToken)
    {
        await cache.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask<byte[]> GetStateVectorAsync(
        DocumentContext context,
        CancellationToken ct = default)
    {
        var container = cache.GetContext(context.DocumentName);

        return await container.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.ReadTransactionOrThrow())
            {
                return transaction.StateVectorV1();
            }
        }).ConfigureAwait(false);
    }

    public async ValueTask<byte[]> GetUpdateAsync(DocumentContext context, byte[] stateVector, CancellationToken ct = default)
    {
        var container = cache.GetContext(context.DocumentName);

        return await container.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.ReadTransactionOrThrow())
            {
                return transaction.StateDiffV1(stateVector);
            }
        }).ConfigureAwait(false);
    }

    public async ValueTask<UpdateResult> ApplyUpdateAsync(DocumentContext context, byte[] stateDiff, CancellationToken ct = default)
    {
        var container = cache.GetContext(context.DocumentName);

        var (result, doc) = await container.ApplyUpdateReturnAsync(doc =>
        {
            var result = new UpdateResult
            {
                Diff = stateDiff
            };

            using (var transaction = doc.WriteTransactionOrThrow())
            {
                result.TransactionUpdateResult = transaction.ApplyV1(stateDiff);
            }

            return (result, doc);
        }).ConfigureAwait(false);

        if (result.Diff != null)
        {
            await callback.OnDocumentChangedAsync(new DocumentChangedEvent
            {
                Context = context,
                Diff = result.Diff,
                Document = doc,
                Source = this,
            }).ConfigureAwait(false);
        }

        return result;
    }

    public async ValueTask UpdateDocAsync(DocumentContext context, Action<Doc> action, CancellationToken ct = default)
    {
        var container = cache.GetContext(context.DocumentName);

        var (diff, doc) = await container.ApplyUpdateReturnAsync(doc =>
        {
            using var subscribeOnce = new SubscribeToUpdatesV1Once(doc);

            action(doc);

            return (subscribeOnce.Update, doc);
        }).ConfigureAwait(false);

        if (diff != null)
        {
            await callback.OnDocumentChangedAsync(new DocumentChangedEvent
            {
                Context = context,
                Diff = diff,
                Document = doc,
                Source = this,
            }).ConfigureAwait(false);
        }
    }

    public async ValueTask PingAsync(DocumentContext context, ulong clock, string? state = null, CancellationToken ct = default)
    {
        if (users.AddOrUpdate(context.DocumentName, context.ClientId, clock, state, out var newState))
        {
            await callback.OnAwarenessUpdatedAsync(new ClientAwarenessEvent
            {
                Context = context,
                ClientClock = clock,
                ClientState = newState,
                Source = this,
            }).ConfigureAwait(false);
        }
    }

    public async ValueTask DisconnectAsync(
        DocumentContext context,
        CancellationToken ct = default)
    {
        if (users.Remove(context.DocumentName, context.ClientId))
        {
            await callback.OnClientDisconnectedAsync(new ClientDisconnectedEvent
            {
                Context = context,
                Source = this,
            }).ConfigureAwait(false);
        }
    }

    public async ValueTask CleanupAsync(
        CancellationToken ct = default)
    {
        foreach (var (clientId, documentName) in users.Cleanup(options.MaxPingTime))
        {
            await callback.OnClientDisconnectedAsync(new ClientDisconnectedEvent
            {
                Context = new DocumentContext(documentName, clientId),
                Source = this,
            }).ConfigureAwait(false);
        }

        cache.RemoveEvictedItems();
    }

    public ValueTask<IReadOnlyDictionary<ulong, ConnectedUser>> GetAwarenessAsync(
        DocumentContext context,
        CancellationToken ct = default)
    {
        return new ValueTask<IReadOnlyDictionary<ulong, ConnectedUser>>(users.GetUsers(context.DocumentName));
    }
}
