using YDotNet.Document;
using YDotNet.Document.Transactions;

namespace YDotNet.Server;

public interface IDocumentManager
{
    ValueTask PingAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask DisconnectAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask UpdateAwarenessAsync(DocumentContext context, string key, object value,
        CancellationToken ct = default);

    ValueTask<byte[]> GetMissingChangesAsync(DocumentContext context, byte[] stateVector,
        CancellationToken ct = default);

    ValueTask<IReadOnlyDictionary<long, ConnectedUser>> GetAwarenessAsync(string roomName,
        CancellationToken ct = default);

    ValueTask<UpdateResult> ApplyUpdateAsync(DocumentContext context, byte[] stateDiff,
        CancellationToken ct = default);

    ValueTask UpdateDocAsync(DocumentContext context, Action<Doc, Transaction> action,
        CancellationToken ct = default);

    ValueTask CleanupAsync(
        CancellationToken ct = default);
}
