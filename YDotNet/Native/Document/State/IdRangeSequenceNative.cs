using System.Runtime.InteropServices;
using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
public class IdRangeSequenceNative
{
    public uint Length { get; init; }

    public nint Sequence { get; init; }

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
