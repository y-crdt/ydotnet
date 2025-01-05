using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YDotNet.Server.Redis.Internal;

namespace YDotNet.Server.Redis;

public sealed class RedisConnection(IOptions<RedisOptions> options, ILogger<RedisConnection> logger) : IDisposable
{
    public Task<IConnectionMultiplexer> Instance { get; } = options.Value.ConnectAsync(new LoggerTextWriter(logger));

    public void Dispose()
    {
        if (Instance.IsCompletedSuccessfully)
        {
            Instance.Result.Close();
        }
    }
}
