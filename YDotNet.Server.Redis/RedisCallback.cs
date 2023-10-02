using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Text.Json;
using YDotNet.Server.Redis.Internal;

namespace YDotNet.Server.Redis;

public sealed class RedisCallback : IDocumentCallback, IHostedService
{
    private readonly Guid senderId = Guid.NewGuid();
    private readonly RedisClusteringOptions options;
    private readonly ILogger<RedisCallback> logger;
    private ISubscriber? subscriber;
    private IDocumentManager? documentManager;

    public RedisCallback(IOptions<RedisClusteringOptions> options, ILogger<RedisCallback> logger)
    {
        this.options = options.Value;
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
        subscriber?.UnsubscribeAll();
        return Task.CompletedTask;
    }

    private async Task HandleMessage(RedisValue value)
    {
        if (documentManager == null)
        {
            return;
        }

        var message = JsonSerializer.Deserialize<Message>(value.ToString());

        if (message == null || message.SenderId == senderId)
        {
            return;
        }

        if (message.DocumentChanged is DocumentChangeMessage changed)
        {
            await documentManager.ApplyUpdateAsync(new DocumentContext
            {
                ClientId = changed.ClientId,
                DocumentName = changed.DocumentName,
                Metadata = senderId
            }, changed.DocumentDiff);
        }

        if (message.Pinged is PingMessage pinged)
        {
            await documentManager.PingAsync(new DocumentContext
            {
                ClientId = pinged.ClientId,
                DocumentName = pinged.DocumentName,
                Metadata = senderId
            }, pinged.ClientClock, pinged.ClientState);
        }

        if (message.ClientDisconnected is PingMessage[] disconnected)
        {
            foreach (var client in disconnected)
            {
                await documentManager.DisconnectAsync(new DocumentContext
                {
                    ClientId = client.ClientId,
                    DocumentName = client.DocumentName,
                    Metadata = senderId
                });
            }
        }
    }

    public async ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        if (subscriber == null)
        {
            return;
        }

        var message = new Message
        {
            SenderId = senderId,
            Pinged = new PingMessage
            {
                ClientId = @event.DocumentContext.ClientId,
                ClientClock = @event.ClientClock,
                ClientState = @event.ClientState,
                DocumentName = @event.DocumentContext.DocumentName,
            },
        };

        var json = JsonSerializer.Serialize(message);

        await subscriber.PublishAsync(options.Channel, json);
    }

    public async ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent[] events)
    {
        if (subscriber == null)
        {
            return;
        }

        var message = new Message
        {
            SenderId = senderId,
            ClientDisconnected = events.Select(x => new PingMessage
            {
                ClientId = x.DocumentContext.ClientId,
                ClientState = null,
                ClientClock = 0,
                DocumentName = x.DocumentContext.DocumentName,
            }).ToArray()
        };

        var json = JsonSerializer.Serialize(message);

        await subscriber.PublishAsync(options.Channel, json);
    }

    public async ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        if (subscriber == null)
        {
            return;
        }

        var message = new Message
        {
            SenderId = senderId,
            DocumentChanged = new DocumentChangeMessage
            {
                ClientId = @event.DocumentContext.ClientId,
                DocumentName = @event.DocumentContext.DocumentName,
                DocumentDiff = @event.Diff
            },
        };

        var json = JsonSerializer.Serialize(message);

        await subscriber.PublishAsync(options.Channel, json);

    }
}
