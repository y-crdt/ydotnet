using YDotNet.Document;

namespace YDotNet.Server.Storage;

public interface IDocumentStorage
{
    ValueTask<byte[]?> GetDocAsync(string name,
        CancellationToken ct = default);

    ValueTask StoreDocAsync(string name, byte[] doc,
        CancellationToken ct = default);
}
