using YDotNet.Native.Document.State;

namespace YDotNet.Document.State;

/// <summary>
///     Represents a single space of clock values, belonging to the same client.
/// </summary>
public sealed record IdRange(uint Start, uint End)
{
    internal static IdRange Create(IdRangeNative native)
    {
        return new IdRange(native.Start, native.End);
    }
}
