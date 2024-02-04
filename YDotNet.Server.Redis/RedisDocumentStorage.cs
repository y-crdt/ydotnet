using Microsoft.Extensions.Options;
using StackExchange.Redis;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Redis;

public sealed class RedisDocumentStorage : IDocumentStorage
{
    private readonly RedisDocumentStorageOptions redisOptions;
    private IDatabase? database;

    public RedisDocumentStorage(IOptions<RedisDocumentStorageOptions> redisOptions, RedisConnection redisConnection)
    {
        this.redisOptions = redisOptions.Value;

        _ = InitializeAsync(redisConnection);
    }

    private async Task InitializeAsync(RedisConnection redisConnection)
    {
        // Use a single task, so that the ordering of registrations does not matter.
        var connection = await redisConnection.Instance.ConfigureAwait(false);

        database = connection.GetDatabase(redisOptions.Database);
    }

    public async ValueTask<byte[]?> GetDocAsync(
        string name,
        CancellationToken ct = default)
    {
        if (database == null)
        {
            return null;
        }

        var item = await database.StringGetAsync(Key(name)).ConfigureAwait(false);

        if (item == RedisValue.Null)
        {
            return null;
        }

        return item;
    }

    public async ValueTask StoreDocAsync(string name, byte[] doc, CancellationToken ct = default)
    {
        if (database == null)
        {
            return;
        }

        await database.StringSetAsync(Key(name), doc, redisOptions.Expiration?.Invoke(name)).ConfigureAwait(false);
    }

    private string Key(string name)
    {
        return redisOptions.Prefix + name;
    }
}
