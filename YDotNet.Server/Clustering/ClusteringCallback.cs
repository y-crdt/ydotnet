using Microsoft.Extensions.Options;
using ProtoBuf;
using YDotNet.Server.Clustering;
using YDotNet.Server.Clustering.Internal;

namespace YDotNet.Server.Redis;

public sealed class ClusteringCallback : IDocumentCallback, IDisposable
{
    private readonly Guid senderId = Guid.NewGuid();
    private readonly ClusteringOptions clusteringOptions;
    private readonly PublishQueue<Message> publishQueue;
    private readonly IPubSub publisher;
    private readonly IDisposable subscription;
    private IDocumentManager? documentManager;

    public ClusteringCallback(IOptions<ClusteringOptions> clusteringOptions, IPubSub publisher)
    {
        this.clusteringOptions = clusteringOptions.Value;

        this.publisher = publisher;
        this.publishQueue = new PublishQueue<Message>(
            this.clusteringOptions.MaxBatchCount,
            this.clusteringOptions.MaxBatchSize,
            (int)this.clusteringOptions.DebounceTime.TotalMilliseconds,
            PublishBatchAsync);

        subscription = publisher.Subscribe(HandleMessage);
    }

    public void Dispose()
    {
        subscription.Dispose();
    }

    public ValueTask OnInitializedAsync(
        IDocumentManager manager)
    {
        // The initialize method is used to prevent circular dependencies between managers and hooks.
        documentManager = manager;
        return default;
    }

    private async Task HandleMessage(byte[] payload)
    {
        if (documentManager == null)
        {
            return;
        }

        var batch = Serializer.Deserialize<Message[]>(payload.AsSpan());

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

        return publishQueue.EnqueueAsync(message, default);
    }

    private async Task PublishBatchAsync(List<Message> batch, CancellationToken ct)
    {
        using var stream = new MemoryStream();

        Serializer.Serialize(stream, batch);

        await publisher.PublishAsync(stream.ToArray(), default);
    }
}
