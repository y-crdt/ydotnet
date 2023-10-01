using System.Collections.Concurrent;
using YDotNet.Document;

namespace YDotNet.Server.Storage;

public sealed class InMemoryDocStorage : IDocumentStorage
{
    private readonly ConcurrentDictionary<string, Doc> docs = new();

    public ValueTask<Doc?> GetDocAsync(string name,
        CancellationToken ct = default)
    {
        docs.TryGetValue(name, out var doc);

        return new ValueTask<Doc?>(doc);
    }

    public ValueTask StoreDocAsync(string name, Doc doc,
        CancellationToken ct = default)
    {
        docs[name] = doc;

        return default;
    }
}
