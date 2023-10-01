using YDotNet.Document;

namespace YDotNet.Server;

public interface IDocumentManager
{
    ValueTask<Doc?> GetDocAsync(string name,
        CancellationToken ct = default);
}
