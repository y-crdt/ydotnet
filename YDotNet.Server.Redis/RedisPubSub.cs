using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YDotNet.Server.Clustering;
using YDotNet.Server.Internal;

namespace YDotNet.Server.Redis;

public sealed class RedisPubSub : IPubSub
{
    private readonly List<Func<byte[], Task>> handlers = new List<Func<byte[], Task>>();
    private readonly RedisClusteringOptions redisOptions;
    private ISubscriber? subscriber;

    public RedisPubSub(IOptions<RedisClusteringOptions> redisOptions, RedisConnection redisConnection)
    {
        this.redisOptions = redisOptions.Value;

        _ = InitializeAsync(redisConnection);
    }

    public async Task InitializeAsync(RedisConnection redisConnection)
    {
        // Use a single task, so that the ordering of registrations does not matter.
        var connection = await redisConnection.Instance;

        subscriber = connection.GetSubscriber();
        subscriber.Subscribe(redisOptions.Channel, async (_, value) =>
        {
            foreach (var handler in handlers)
            {
                byte[]? payload = value;

                if (payload != null)
                {
                    await handler(payload);
                }
            }
        });
    }

    public void Dispose()
    {
        subscriber?.UnsubscribeAll();
    }

    public IDisposable Subscribe(Func<byte[], Task> handler)
    {
        handlers.Add(handler);

        return new DelegateDisposable(() =>
        {
            handlers.Remove(handler);
        });
    }

    public async Task PublishAsync(byte[] payload, CancellationToken ct)
    {
        if (subscriber == null)
        {
            return;
        }

        await subscriber.PublishAsync(redisOptions.Channel, payload);
    }
}
