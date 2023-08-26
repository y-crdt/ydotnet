using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

internal struct IdRangeNative
{
    public uint Start { get; }

    public uint End { get; }

    public IdRange ToIdRange()
    {
        return new IdRange(Start, End);
    }
}
