using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells;

[StructLayout(LayoutKind.Explicit)]
internal struct InputContent
{
    [field: FieldOffset(offset: 0)]
    public nint Doc { get; }
}
