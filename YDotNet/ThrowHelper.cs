namespace YDotNet;

internal static class ThrowHelper
{
    public static void InternalError()
    {
        throw new YDotNetException("Operation failed. Lib returns null pointer without further details.");
    }

    public static void PendingTransaction()
    {
        throw new YDotNetException("Failed to open a transaction. Probably because another transaction is still pending.");
    }

    public static void YDotnet(string message)
    {
        throw new YDotNetException(message);
    }

    public static void ArgumentException(string message, string paramName)
    {
        throw new ArgumentException(message, paramName);
    }
}
