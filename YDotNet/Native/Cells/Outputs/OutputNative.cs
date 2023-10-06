using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Outputs;

[StructLayout(LayoutKind.Explicit, Size = Size)]
internal struct OutputNative
{
    internal const int Size = 16;

    [FieldOffset(offset: 0)]
    public sbyte Tag;

    [FieldOffset(offset: 1)]
    public uint Length;
}
