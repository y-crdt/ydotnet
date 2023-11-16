using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YDotNet.Server.Redis.Internal;

namespace YDotNet.Server.Redis;

public sealed class RedisConnection : IDisposable
{
    public RedisConnection(IOptions<RedisOptions> options, ILogger<RedisConnection> logger)
    {
        Instance = options.Value.ConnectAsync(new LoggerTextWriter(logger));
    }

    public Task<IConnectionMultiplexer> Instance { get; }

    public void Dispose()
    {
        if (Instance.IsCompletedSuccessfully)
        {
            Instance.Result.Close();
        }
    }
}
