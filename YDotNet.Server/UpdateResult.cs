using YDotNet.Document.Transactions;

namespace YDotNet.Server;

public sealed class UpdateResult
{
    public TransactionUpdateResult TransactionUpdateResult { get; set; }

    public bool IsSkipped { get; set; }

    public byte[]? Update { get; set; }
}
