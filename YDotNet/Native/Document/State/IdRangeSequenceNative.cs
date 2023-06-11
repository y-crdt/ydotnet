using System.Runtime.InteropServices;
using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal struct IdRangeSequenceNative
{
    public uint Length { get; }

    public nint Sequence { get; }

    public IdRange[] ToIdRanges()
    {
        var sequence = new IdRange[Length];

        for (var i = 0; i < Length; i++)
        {
            sequence[i] = Marshal.PtrToStructure<IdRange>(Sequence + i * Marshal.SizeOf<IdRange>());
        }

        return sequence;
    }
}
