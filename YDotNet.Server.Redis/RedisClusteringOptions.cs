using StackExchange.Redis;

namespace YDotNet.Server.Redis;

public sealed class RedisClusteringOptions
{
    public RedisChannel Channel { get; set; } = RedisChannel.Literal("YDotNet");

    public TimeSpan DebounceTime { get; set; } = TimeSpan.FromMilliseconds(500);

    public int MaxBatchCount { get; set; } = 100;

    public int MaxBatchSize { get; set; } = 1024 * 1024;

    public ConfigurationOptions? Configuration { get; set; }

    public Func<TextWriter, Task<IConnectionMultiplexer>>? ConnectionFactory { get; set; }

    internal async Task<IConnectionMultiplexer> ConnectAsync(TextWriter log)
    {
        if (ConnectionFactory != null)
        {
            return await ConnectionFactory(log);
        }

        if (Configuration != null)
        {
            return await ConnectionMultiplexer.ConnectAsync(Configuration, log);
        }

        throw new InvalidOperationException("Either configuration or connection factory must be set.");
    }
}
