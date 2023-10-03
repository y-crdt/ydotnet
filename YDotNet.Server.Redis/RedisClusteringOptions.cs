using StackExchange.Redis;

namespace YDotNet.Server.Redis;

public sealed class RedisClusteringOptions
{
    public RedisChannel Channel { get; set; } = RedisChannel.Literal("YDotNet");

    public TimeSpan DebounceTime { get; set; } = TimeSpan.FromMilliseconds(500);

    public int MaxBatchCount { get; set; } = 100;

    public int MaxBatchSize { get; set; } = 1024 * 1024;
}
