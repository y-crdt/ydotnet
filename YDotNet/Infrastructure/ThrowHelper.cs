namespace YDotNet.Infrastructure;

internal static class ThrowHelper
{
    public static void Null()
    {
        throw new YDotNetException("Operation failed. The yffi library returned null without further details.");
    }

    public static void PendingTransaction()
    {
        throw new YDotNetException("Failed to open a transaction, probably because another transaction is still open.");
    }
}
