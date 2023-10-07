using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Inputs;

// The size has to be 24 here so that the whole data of the input cell is written/read correctly over the C FFI.
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal struct InputNative
{
    [field: FieldOffset(offset: 0)]
    public sbyte Tag { get; }

    [field: FieldOffset(offset: 4)]
    public uint Length { get; }
}
