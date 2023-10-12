using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct IdRangeSequenceNative
{
    public uint SequenceLength { get; }

    public nint SequenceHandle { get; }

    public IdRangeNative[] Sequence()
    {
        return MemoryReader.ReadStructArray<IdRangeNative>(SequenceHandle, SequenceLength);
    }
}
