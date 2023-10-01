using System.Collections.Concurrent;
using YDotNet.Document;

namespace YDotNet.Server.Storage;

public sealed class InMemoryDocStorage : IDocumentStorage
{
    private readonly ConcurrentDictionary<string, byte[]> docs = new();

    public ValueTask<byte[]?> GetDocAsync(string name,
        CancellationToken ct = default)
    {
        docs.TryGetValue(name, out var doc);

        return new ValueTask<byte[]?>(doc);
    }

    public ValueTask StoreDocAsync(string name, byte[] doc,
        CancellationToken ct = default)
    {
        docs[name] = doc;

        return default;
    }
}
