using Microsoft.Extensions.Hosting;
using YDotNet.Document;
using YDotNet.Document.Transactions;

namespace YDotNet.Server;

public interface IDocumentManager : IHostedService
{
    ValueTask PingAsync(DocumentContext context, long clock, string? state = null,
        CancellationToken ct = default);

    ValueTask DisconnectAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask<byte[]> GetUpdateAsync(DocumentContext context, byte[] stateVector,
        CancellationToken ct = default);

    ValueTask<byte[]> GetStateVectorAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask<IReadOnlyDictionary<long, ConnectedUser>> GetAwarenessAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask<UpdateResult> ApplyUpdateAsync(DocumentContext context, byte[] stateDiff,
        CancellationToken ct = default);

    ValueTask UpdateDocAsync(DocumentContext context, Action<Doc> action,
        CancellationToken ct = default);

    ValueTask CleanupAsync(
        CancellationToken ct = default);
}