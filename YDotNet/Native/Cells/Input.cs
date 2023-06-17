using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells;

[StructLayout(LayoutKind.Explicit, Size = 24)]
internal struct Input
{
    [field: FieldOffset(offset: 0)]
    public sbyte Tag { get; }

    [field: FieldOffset(offset: 4)]
    public uint Length { get; }

    [field: FieldOffset(offset: 8)]
    public InputContent Content { get; }
}
