using Microsoft.Extensions.Hosting;
using YDotNet.Document;

namespace YDotNet.Server;

public interface IDocumentManager : IHostedService
{
    ValueTask PingAsync(DocumentContext context, ulong clock, string? state = null,
        CancellationToken ct = default);

    ValueTask DisconnectAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask<byte[]> GetUpdateAsync(DocumentContext context, byte[] stateVector,
        CancellationToken ct = default);

    ValueTask<byte[]> GetStateVectorAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask<IReadOnlyDictionary<ulong, ConnectedUser>> GetAwarenessAsync(DocumentContext context,
        CancellationToken ct = default);

    ValueTask<UpdateResult> ApplyUpdateAsync(DocumentContext context, byte[] stateDiff,
        CancellationToken ct = default);

    ValueTask UpdateDocAsync(DocumentContext context, Action<Doc> action,
        CancellationToken ct = default);

    ValueTask CleanupAsync(
        CancellationToken ct = default);
}
