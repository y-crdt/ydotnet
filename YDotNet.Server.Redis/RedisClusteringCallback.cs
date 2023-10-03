using Microsoft.Extensions.Options;
using ProtoBuf;
using StackExchange.Redis;
using YDotNet.Server.Redis.Internal;

namespace YDotNet.Server.Redis;

public sealed class RedisClusteringCallback : IDocumentCallback, IDisposable
{
    private readonly Guid senderId = Guid.NewGuid();
    private readonly RedisClusteringOptions redisOptions;
    private readonly PublishQueue<Message> subscriberQueue;
    private ISubscriber? subscriber;
    private IDocumentManager? documentManager;

    public RedisClusteringCallback(IOptions<RedisClusteringOptions> redisOptions, RedisConnection redisConnection)
    {
        this.redisOptions = redisOptions.Value;

        subscriberQueue = new PublishQueue<Message>(
            this.redisOptions.MaxBatchCount,
            this.redisOptions.MaxBatchSize,
            (int)this.redisOptions.DebounceTime.TotalMilliseconds,
            PublishBatchAsync);

        _ = InitializeAsync(redisConnection);
    }

    public async Task InitializeAsync(RedisConnection redisConnection)
    {
        // Use a single task, so that the ordering of registrations does not matter.
        var connection = await redisConnection.Instance;

        subscriber = connection.GetSubscriber();
        subscriber.Subscribe(redisOptions.Channel, async (_, value) =>
        {
            await HandleMessage(value);
        });
    }

    public void Dispose()
    {
        subscriberQueue.Dispose();
        subscriber?.UnsubscribeAll();
    }

    public ValueTask OnInitializedAsync(
        IDocumentManager manager)
    {
        // The initialize method is used to prevent circular dependencies between managers and hooks.
        documentManager = manager;
        return default;
    }

    private async Task HandleMessage(RedisValue value)
    {
        if (documentManager == null)
        {
            return;
        }

        var batch = Serializer.Deserialize<Message[]>(value);

        if (batch == null)
        {
            return;
        }

        foreach (var message in batch)
        {
            if (message.SenderId == senderId)
            {
                continue;
            }

            var context = new DocumentContext(message.DocumentName, message.ClientId)
            {
                Metadata = senderId
            };

            if (message.DocumentChanged is DocumentChangeMessage changed)
            {
                await documentManager.ApplyUpdateAsync(context, changed.DocumentDiff);
            }

            if (message.ClientPinged is ClientPingMessage pinged)
            {
                await documentManager.PingAsync(context, pinged.ClientClock, pinged.ClientState);
            }

            if (message.ClientDisconnected is not null)
            {
                await documentManager.DisconnectAsync(context);
            }
        }       
    }

    public ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        var message = new Message
        {
            ClientId = @event.Context.ClientId,
            ClientPinged = new ClientPingMessage
            {
                ClientClock = @event.ClientClock,
                ClientState = @event.ClientState,
            },
            DocumentName = @event.Context.DocumentName,
            SenderId = senderId,
        };

        return subscriberQueue.EnqueueAsync(message, default);
    }

    public ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent @event)
    {
        var message = new Message
        {
            ClientId = @event.Context.ClientId,
            ClientDisconnected = new ClientDisconnectMessage(),
            DocumentName = @event.Context.DocumentName,
            SenderId = senderId,
        };

        return subscriberQueue.EnqueueAsync(message, default);
    }

    public ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        var message = new Message
        {
            ClientId = @event.Context.ClientId,
            DocumentName = @event.Context.DocumentName,
            DocumentChanged = new DocumentChangeMessage
            {
                DocumentDiff = @event.Diff
            },
            SenderId = senderId
        };

        return subscriberQueue.EnqueueAsync(message, default);
    }

    private async Task PublishBatchAsync(List<Message> batch, CancellationToken ct)
    {
        if (subscriber == null)
        {
            return;
        }

        using var stream = new MemoryStream();

        Serializer.Serialize(stream, batch);

        await subscriber.PublishAsync(redisOptions.Channel, stream.ToArray());
    }
}
