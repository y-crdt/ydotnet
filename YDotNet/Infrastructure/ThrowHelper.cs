namespace YDotNet.Infrastructure;

internal static class ThrowHelper
{
    public static void Null()
    {
        YDotNetException("Operation failed. The yffi library returned null without further details.");
    }

    public static void PendingTransaction()
    {
        YDotNetException("Failed to open a transaction, probably because another transaction is still open.");
    }

    public static void YDotNetException(string message)
    {
        throw new YDotNetException(message);
    }
}
