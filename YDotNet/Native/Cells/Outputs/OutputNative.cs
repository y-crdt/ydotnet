using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Outputs;

[StructLayout(LayoutKind.Explicit, Size = 16)]
internal struct OutputNative
{
    [field: FieldOffset(offset: 0)]
    public sbyte Tag { get; }

    [field: FieldOffset(offset: 4)]
    public uint Length { get; }
}
