using YDotNet.Document;

namespace YDotNet.Server.Storage;

public interface IDocumentStorage
{
    ValueTask<Doc?> GetDocAsync(string name,
        CancellationToken ct = default);

    ValueTask StoreDocAsync(string name, Doc doc,
        CancellationToken ct = default);
}
