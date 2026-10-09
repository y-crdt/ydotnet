namespace YDotNet.Infrastructure;

internal static class ThrowHelper
{
    public static void Null()
    {
        throw new YDotNetException("Operation failed. The yffi library returned null without further details.");
    }

    public static void CheckInsertIndex(uint index, uint length)
    {
        if (index > length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                $"The index must be between 0 and the current length ({length}).");
        }
    }

    public static void CheckRange(uint index, uint count, uint length)
    {
        if (index > length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(index),
                index,
                $"The index must be between 0 and the current length ({length}).");
        }

        if ((ulong)index + count > length)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                $"The range starting at {index} must not go over the current length ({length}).");
        }
    }

    public static void PendingTransaction()
    {
        throw new YDotNetException("Failed to open a transaction, probably because another transaction is still open.");
    }
}
