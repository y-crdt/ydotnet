using YDotNet.Document;
using YDotNet.Document.Transactions;

namespace YDotNet.Server.Internal;

internal static class Extensions
{
    public static Transaction ReadTransactionOrThrow(this Doc doc)
    {
        return doc.ReadTransaction() ?? throw new InvalidOperationException("Failed to open transaction.");
    }

    public static Transaction WriteTransactionOrThrow(this Doc doc)
    {
        return doc.WriteTransaction() ?? throw new InvalidOperationException("Failed to open transaction.");
    }
}
