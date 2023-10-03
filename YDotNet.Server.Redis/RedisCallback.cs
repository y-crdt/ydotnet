using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ProtoBuf;
using StackExchange.Redis;
using System.Text.Json;
using YDotNet.Server.Redis.Internal;

namespace YDotNet.Server.Redis;

public sealed class RedisCallback : IDocumentCallback, IHostedService
{
    private readonly Guid senderId = Guid.NewGuid();
    private readonly RedisClusteringOptions options;
    private readonly ILogger<RedisCallback> logger;
    private readonly PublishQueue<Message> queue;
    private ISubscriber? subscriber;
    private IDocumentManager? documentManager;

    public RedisCallback(IOptions<RedisClusteringOptions> options, ILogger<RedisCallback> logger)
    {
        this.options = options.Value;

        queue = new PublishQueue<Message>(
            this.options.MaxBatchCount,
            this.options.MaxBatchSize,
            (int)this.options.DebounceTime.TotalMilliseconds,
            PublishBatchAsync);

        this.logger = logger;
    }

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        var connection = await options.ConnectAsync(new LoggerTextWriter(logger));

        // Is only needed for topics, but it has only minor costs.
        subscriber = connection.GetSubscriber();
        subscriber.Subscribe(options.Channel, async (_, value) =>
        {
            await HandleMessage(value);
        });
    }

    public ValueTask OnInitializedAsync(
        IDocumentManager manager)
    {
        documentManager = manager;
        return default;
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        queue.Dispose();

        subscriber?.UnsubscribeAll();
        return Task.CompletedTask;
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

        return queue.EnqueueAsync(message, default);
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

        return queue.EnqueueAsync(message, default);
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

        return queue.EnqueueAsync(message, default);
    }

    private async Task PublishBatchAsync(List<Message> batch, CancellationToken ct)
    {
        if (subscriber == null)
        {
            return;
        }

        using var stream = new MemoryStream();

        Serializer.Serialize(stream, batch);

        await subscriber.PublishAsync(options.Channel, stream.ToArray());
    }
}
