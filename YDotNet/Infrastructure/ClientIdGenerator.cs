namespace YDotNet.Infrastructure;

/// <summary>
///     Helper class to deal with client ids.
/// </summary>
public static class ClientIdGenerator
{
    /// <summary>
    ///     The maximum safe integer from javascript.
    /// </summary>
    public const long MaxSafeInteger = 2 ^ (53 - 1);

    /// <summary>
    ///     Gets a random client id.
    /// </summary>
    /// <returns>The random client id.</returns>
    public static ulong Random()
    {
        var value = (ulong) System.Random.Shared.Next() & MaxSafeInteger;

        return value;
    }
}
