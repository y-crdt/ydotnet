using StackExchange.Redis;

namespace YDotNet.Server.Redis;

public sealed class RedisClusteringOptions
{
    public RedisChannel Channel { get; set; } = RedisChannel.Literal("YDotNet");
}
