using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Server.Internal;
using YDotNet.Server.Storage;

#pragma warning disable IDE0063 // Use simple 'using' statement

namespace YDotNet.Server;

public sealed class DefaultDocumentManager : IDocumentManager
{
    private readonly ConnectedUsers users = new();
    private readonly DocumentManagerOptions options;
    private readonly DocumentContainerCache containers;
    private readonly CallbackInvoker callbacks;

    public DefaultDocumentManager(IDocumentStorage documentStorage, IEnumerable<IDocumentCallback> callbacks,
        IOptions<DocumentManagerOptions> options, ILogger<DefaultDocumentManager> logger)
    {
        this.options = options.Value;
        this.callbacks = new CallbackInvoker(callbacks, logger);

        containers = new DocumentContainerCache(documentStorage, options.Value);
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        await callbacks.OnInitializedAsync(this);
    }

    public async Task StopAsync(
        CancellationToken cancellationToken)
    {
        await containers.DisposeAsync();
    }

    public async ValueTask<(byte[] Update, byte[] StateVector)> GetMissingChangesAsync(DocumentContext context, byte[] stateVector,
        CancellationToken ct = default)
    {
        var container = containers.GetContext(context.DocumentName);

        return await container.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.ReadTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be created.");
                }

                byte[] update;
                if (stateVector.Length == 0)
                {
                    update = stateVector;
                }
                else
                {
                    update = transaction.StateDiffV2(stateVector);
                }

                return (update, transaction.StateVectorV1());
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

            using (var transaction = doc.WriteTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be created.");
                }

                result.TransactionUpdateResult = transaction.ApplyV2(stateDiff);
            }

            return (result, doc);
        }, async doc =>
        {
            await callbacks.OnDocumentChangingAsync(new DocumentChangeEvent
            {
                DocumentContext = context,
                Document = doc,
                DocumentManager = this,
            });
        });

        if (result.Diff != null)
        {
            await callbacks.OnDocumentChangedAsync(new DocumentChangedEvent
            {
                Diff = result.Diff,
                Document = doc,
                DocumentContext = context,
                DocumentManager = this,
            });
        }

        return result;
    }

    public async ValueTask UpdateDocAsync(DocumentContext context, Action<Doc, Transaction> action,
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

            using (var transaction = doc.WriteTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be created.");
                }

                action(doc, transaction);
            }

            doc.UnobserveUpdatesV2(subscription);
            return (diff, doc);
        }, async doc =>
        {
            await callbacks.OnDocumentChangingAsync(new DocumentChangeEvent
            {
                Document = doc,
                DocumentContext = context,
                DocumentManager = this,
            });
        });

        if (diff != null)
        {
            await callbacks.OnDocumentChangedAsync(new DocumentChangedEvent
            {
                Diff = diff,
                Document = doc,
                DocumentContext = context,
                DocumentManager = this,
            });
        }
    }

    public async ValueTask PingAsync(DocumentContext context, long clock, string? state = null,
        CancellationToken ct = default)
    {
        if (users.AddOrUpdate(context.DocumentName, context.ClientId, clock, state, out var newState))
        {
            await callbacks.OnAwarenessUpdatedAsync(new ClientAwarenessEvent
            {
                DocumentContext = context,
                DocumentManager = this,
                ClientClock = clock,
                ClientState = newState
            });
        }
    }

    public async ValueTask DisconnectAsync(DocumentContext context,
        CancellationToken ct = default)
    {
        if (users.Remove(context.DocumentName, context.ClientId))
        {
            await callbacks.OnClientDisconnectedAsync(new[] 
            { 
                new ClientDisconnectedEvent
                {
                    DocumentContext = context,
                    DocumentManager = this,
                }
            });
        }
    }

    public async ValueTask CleanupAsync(
        CancellationToken ct = default)
    {
        var removedUsers = users.Cleanup(options.MaxPingTime).ToList();

        if (removedUsers.Count > 0)
        {
            await callbacks.OnClientDisconnectedAsync(removedUsers.Select(x =>
            {
                return new ClientDisconnectedEvent
                {
                    DocumentContext = new DocumentContext { ClientId = x.ClientId, DocumentName = x.DocumentName },
                    DocumentManager = this,
                };
            }).ToArray());
        }

        containers.RemoveEvictedItems();
    }

    public ValueTask<IReadOnlyDictionary<long, ConnectedUser>> GetAwarenessAsync(string roomName,
        CancellationToken ct = default)
    {
        return new ValueTask<IReadOnlyDictionary<long, ConnectedUser>>(users.GetUsers(roomName));
    }
}
