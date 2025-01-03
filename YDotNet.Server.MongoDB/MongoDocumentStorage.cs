namespace YDotNet.Server.MongoDB;

using global::MongoDB.Driver;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using YDotNet.Server.Storage;

public sealed class MongoDocumentStorage(IMongoClient mongoClient, IOptions<MongoDocumentStorageOptions> options) : IDocumentStorage, IHostedService
{
    private readonly UpdateOptions upsert = new() { IsUpsert = true };
    private readonly MongoDocumentStorageOptions options = options.Value;
    private IMongoCollection<DocumentEntity>? collection;

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    public async Task StartAsync(
        CancellationToken cancellationToken)
    {
        var database = mongoClient.GetDatabase(options.DatabaseName);

        collection = database.GetCollection<DocumentEntity>(options.CollectionName);

        await collection.Indexes.CreateOneAsync(
            new CreateIndexModel<DocumentEntity>(
                Builders<DocumentEntity>.IndexKeys.Ascending(x => x.Expiration),
                new CreateIndexOptions
                {
                    ExpireAfter = TimeSpan.Zero,
                }),
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async ValueTask<byte[]?> GetDocAsync(string name, CancellationToken ct = default)
    {
        if (collection == null)
        {
            return null;
        }

        var document = await collection.Find(x => x.Id == name).FirstOrDefaultAsync(ct).ConfigureAwait(false);

        return document?.Data;
    }

    public async ValueTask StoreDocAsync(string name, byte[] doc, CancellationToken ct = default)
    {
        if (collection == null)
        {
            return;
        }

        await collection.UpdateOneAsync(
            x => x.Id == name,
            Builders<DocumentEntity>.Update
                .Set(x => x.Data, doc)
                .Set(x => x.Expiration, GetExpiration(name)),
            upsert,
            ct).ConfigureAwait(false);
    }

    private DateTime? GetExpiration(string name)
    {
        var relative = options.Expiration?.Invoke(name);

        if (relative == null)
        {
            return null;
        }

        return Clock() + relative.Value;
    }
}
