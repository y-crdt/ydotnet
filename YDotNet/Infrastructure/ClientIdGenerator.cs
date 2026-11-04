namespace YDotNet.Infrastructure;

/// <summary>
///     Helper class to deal with client ids.
/// </summary>
public static class ClientIdGenerator
{
    /// <summary>
    ///     The maximum safe integer from javascript (2^53 - 1).
    /// </summary>
    public const ulong MaxSafeInteger = (1UL << 53) - 1;

    /// <summary>
    ///     Gets a random client id.
    /// </summary>
    /// <returns>The random client id.</returns>
    public static ulong Random()
    {
        // Random.Shared.Next() returns only 31 bits (0 .. int.MaxValue), which under-fills the
        // safe-integer range; draw across the full [0, MaxSafeInteger] range instead.
        return (ulong)System.Random.Shared.NextInt64(0, (long)MaxSafeInteger + 1);
    }
}
