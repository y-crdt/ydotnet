using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using YDotNet.Server.Storage;

namespace YDotNet.Server.MongoDB;

public sealed class MongoDocumentStorage : IDocumentStorage, IHostedService
{
    private readonly UpdateOptions Upsert = new UpdateOptions { IsUpsert = true };
    private readonly MongoDocumentStorageOptions options;
    private readonly IMongoClient mongoClient;
    private IMongoCollection<DocumentEntity>? collection;

    public Func<DateTime> Clock = () => DateTime.UtcNow;

    public MongoDocumentStorage(IMongoClient mongoClient, IOptions<MongoDocumentStorageOptions> options)
    {
        this.options = options.Value;
        this.mongoClient = mongoClient;
    }

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
                    ExpireAfter = TimeSpan.Zero
                }),
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async ValueTask<byte[]?> GetDocAsync(string name,
        CancellationToken ct = default)
    {
        if (collection == null)
        {
            return null;
        }

        var document = await collection.Find(x => x.Id == name).FirstOrDefaultAsync(ct);

        return document?.Data;
    }

    public async ValueTask StoreDocAsync(string name, byte[] doc,
        CancellationToken ct = default)
    {
        if (collection == null)
        {
            return;
        }

        await collection.UpdateOneAsync(x => x.Id == name,
            Builders<DocumentEntity>.Update
                .Set(x => x.Data, doc)
                .Set(x => x.Expiration, GetExpiration(name)),
            Upsert,
            ct);
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
