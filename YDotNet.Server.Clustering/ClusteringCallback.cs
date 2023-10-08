using Microsoft.Extensions.Options;
using ProtoBuf;
using StackExchange.Redis;
using YDotNet.Server.Clustering.Internal;

namespace YDotNet.Server.Clustering;

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

            switch (message.Type)
            {
                case MessageType.ClientPinged:
                    await documentManager.PingAsync(context, message.ClientClock, message.ClientState);
                    break;
                case MessageType.ClientDisconnected:
                    await documentManager.DisconnectAsync(context);
                    break;
                case MessageType.Update when message.Data != null:
                    await documentManager.ApplyUpdateAsync(context, message.Data);
                    break;
                case MessageType.SyncStep2 when message.Data != null:
                    await documentManager.ApplyUpdateAsync(context, message.Data);
                    break;
                case MessageType.SyncStep1 when message.Data != null:
                    await SendSync2Async(context, message.Data);
                    break;
                case MessageType.AwarenessRequested:
                    foreach (var (id, user) in await documentManager.GetAwarenessAsync(context))
                    {
                        var userContext = context with { ClientId = id };

                        await SendAwarenessAsync(userContext, user.ClientState, user.ClientClock);
                    }
                    break;
            }
        }
    }

    public ValueTask OnDocumentLoadedAsync(DocumentLoadEvent @event)
    {
        // Run these callbacks in another thread because it could cause deadlocks if it would interact with the same document.
        _ = Task.Run(async () =>
        {
            await SendAwarenessRequest(@event.Context);
            await SendSync1Async(@event.Context);
        });

        return default;
    }

    public async ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        await SendAwarenessAsync(@event.Context, @event.ClientState, @event.ClientClock);
    }

    public ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent @event)
    {
        var m = new Message { Type = MessageType.ClientDisconnected };

        return EnqueueAsync(m, @event.Context);
    }

    public ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        var m = new Message { Type = MessageType.Update, Data = @event.Diff };

        return EnqueueAsync(m, @event.Context);
    }

    private ValueTask SendAwarenessAsync(DocumentContext context, string? state, long clock)
    {
        var m = new Message { Type = MessageType.ClientPinged, ClientState = state, ClientClock = clock };

        return EnqueueAsync(m, context);
    }

    private async ValueTask SendSync1Async(DocumentContext context)
    {
        var state = await documentManager!.GetStateVectorAsync(context);

        var m = new Message { Type = MessageType.SyncStep1, Data = state };

        await EnqueueAsync(m, context);
    }

    private async ValueTask SendSync2Async(DocumentContext context, byte[] stateVector)
    {
        var state = await documentManager!.GetUpdateAsync(context, stateVector);

        var m = new Message { Type = MessageType.SyncStep2, Data = state };

        await EnqueueAsync(m, context);
    }

    private ValueTask SendAwarenessRequest(DocumentContext context)
    {
        var m = new Message { Type = MessageType.AwarenessRequested };

        return EnqueueAsync(m, context);
    }

    private ValueTask EnqueueAsync(Message message, DocumentContext context)
    {
        message.ClientId = context.ClientId;
        message.DocumentName = context.DocumentName;
        message.SenderId = senderId;

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
