namespace YDotNet.Infrastructure;

/// <summary>
/// Helper class to deal with client ids.
/// </summary>
public sealed class ClientId
{
    /// <summary>
    /// The maximum safe integer from javascript.
    /// </summary>
    public const ulong MaxSafeInteger = 2 ^ 53 - 1;

    /// <summary>
    /// Gets a random client id.
    /// </summary>
    /// <returns>The random client id.</returns>
    public static ulong GetRandom()
    {
        var value = ((ulong)Random.Shared.NextInt64() + 1000) & MaxSafeInteger;

        return value;
    }
}
