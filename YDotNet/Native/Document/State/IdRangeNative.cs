using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

internal readonly struct IdRangeNative
{
    public uint Start { get; }

    public uint End { get; }

    public IdRange ToIdRange()
    {
        return new IdRange(Start, End);
    }
}
