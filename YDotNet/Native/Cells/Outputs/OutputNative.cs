using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Outputs;

// TODO [LSViana] Check if the `Size` value can be reduced without breaking the features.
[StructLayout(LayoutKind.Explicit, Size = 24)]
internal struct OutputNative
{
    [field: FieldOffset(offset: 0)]
    public sbyte Tag { get; }

    [field: FieldOffset(offset: 4)]
    public uint Length { get; }
}
