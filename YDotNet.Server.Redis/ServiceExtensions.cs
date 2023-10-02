using Microsoft.Extensions.Hosting;
using YDotNet.Server.Redis;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddRedisClustering(this YDotnetRegistration registration, Action<RedisClusteringOptions> configure)
    {
        registration.Services.Configure(configure);
        registration.Services.AddSingleton<RedisCallback>();

        registration.Services.AddSingleton<IHostedService>(x =>
            x.GetRequiredService<RedisCallback>());

        return registration;
    }
}
