using System.Collections.Concurrent;

namespace YDotNet.Server.Storage;

public sealed class InMemoryDocumentStorage : IDocumentStorage
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
