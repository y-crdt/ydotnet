using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Server.Internal;
using YDotNet.Server.Storage;
using IDocumentCallbacks = System.Collections.Generic.IEnumerable<YDotNet.Server.IDocumentCallback>;

#pragma warning disable IDE0063 // Use simple 'using' statement

namespace YDotNet.Server;

public sealed class DefaultDocumentManager : IDocumentManager
{
    private readonly ConnectedUsers users = new();
    private readonly DocumentManagerOptions options;
    private readonly DocumentContainerCache containers;
    private readonly CallbackInvoker callback;

    public DefaultDocumentManager(
        IDocumentStorage documentStorage,
        IDocumentCallbacks callbacks,
        IOptions<DocumentManagerOptions> options,
        ILogger<DefaultDocumentManager> logger)
    {
        this.options = options.Value;
        this.callback = new CallbackInvoker(callbacks, logger);

        containers = new DocumentContainerCache(documentStorage, this.callback, this, options.Value);
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        await callback.OnInitializedAsync(this);
    }

    public async Task StopAsync(
        CancellationToken cancellationToken)
    {
        await containers.DisposeAsync();
    }

    public async ValueTask<byte[]> GetStateAsync(DocumentContext context,
        CancellationToken ct = default)
    {
        var container = containers.GetContext(context.DocumentName);

        return await container.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.ReadTransactionOrThrow())
            {
                return transaction.StateVectorV1();
            }
        }, null);
    }

    public async ValueTask<byte[]> GetStateAsUpdateAsync(DocumentContext context, byte[] stateVector,
        CancellationToken ct = default)
    {
        var container = containers.GetContext(context.DocumentName);

        return await container.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.ReadTransactionOrThrow())
            {
                return transaction.StateDiffV1(stateVector);
            }
        }, null);
    }

    public async ValueTask<UpdateResult> ApplyUpdateAsync(DocumentContext context, byte[] stateDiff,
        CancellationToken ct = default)
    {
        var container = containers.GetContext(context.DocumentName);

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
        }, async doc =>
        {
            await callback.OnDocumentChangingAsync(new DocumentChangeEvent
            {
                Context = context,
                Document = doc,
                Source = this,
            });
        });

        if (result.Diff != null)
        {
            await callback.OnDocumentChangedAsync(new DocumentChangedEvent
            {
                Context = context,
                Diff = result.Diff,
                Document = doc,
                Source = this,
            });
        }

        return result;
    }

    public async ValueTask UpdateDocAsync(DocumentContext context, Action<Doc> action,
        CancellationToken ct = default)
    {
        var container = containers.GetContext(context.DocumentName);

        var (diff, doc) = await container.ApplyUpdateReturnAsync(doc =>
        {
            byte[]? diff = null;
            var subscription = doc.ObserveUpdatesV1(@event =>
            {
                diff = @event.Update;
            });

            action(doc);

            doc.UnobserveUpdatesV2(subscription);
            return (diff, doc);
        }, async doc =>
        {
            await callback.OnDocumentChangingAsync(new DocumentChangeEvent
            {
                Context = context,
                Document = doc,
                Source = this,
            });
        });

        if (diff != null)
        {
            await callback.OnDocumentChangedAsync(new DocumentChangedEvent
            {
                Context = context,
                Diff = diff,
                Document = doc,
                Source = this,
            });
        }
    }

    public async ValueTask PingAsync(DocumentContext context, long clock, string? state = null,
        CancellationToken ct = default)
    {
        if (users.AddOrUpdate(context.DocumentName, context.ClientId, clock, state, out var newState))
        {
            await callback.OnAwarenessUpdatedAsync(new ClientAwarenessEvent
            {
                Context = context,
                ClientClock = clock,
                ClientState = newState,
                Source = this,
            });
        }
    }

    public async ValueTask DisconnectAsync(DocumentContext context,
        CancellationToken ct = default)
    {
        if (users.Remove(context.DocumentName, context.ClientId))
        {
            await callback.OnClientDisconnectedAsync(new ClientDisconnectedEvent
            {
                Context = context,
                Source = this,
            });
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
            });
        }

        containers.RemoveEvictedItems();
    }

    public ValueTask<IReadOnlyDictionary<long, ConnectedUser>> GetAwarenessAsync(DocumentContext context,
        CancellationToken ct = default)
    {
        return new ValueTask<IReadOnlyDictionary<long, ConnectedUser>>(users.GetUsers(context.DocumentName));
    }
}
