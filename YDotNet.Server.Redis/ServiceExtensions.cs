using YDotNet.Server;
using YDotNet.Server.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static void AddYDotNetRedisClustering(IServiceCollection services, Action<RedisClusteringOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IDocumentCallback, RedisCallback>();
    }
}
