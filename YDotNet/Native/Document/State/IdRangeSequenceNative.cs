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
        var idRangeSize = Marshal.SizeOf<IdRangeNative>();
        var sequence = new IdRange[Length];

        for (var i = 0; i < Length; i++)
        {
            var idRange = Marshal.PtrToStructure<IdRangeNative>(Sequence + i * idRangeSize);
            sequence[i] = idRange.ToIdRange();
        }

        return sequence;
    }
}
