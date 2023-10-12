using System.Runtime.InteropServices;
using YDotNet.Document.State;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct IdRangeSequenceNative
{
    public uint Length { get; }

    public nint Sequence { get; }

    public IdRange[] ToIdRanges()
    {
        var rangeNatives = MemoryReader.ReadStructArray<IdRangeNative>(Sequence, Length);

        return rangeNatives.Select(x => x.ToIdRange()).ToArray(); ;
    }
}
