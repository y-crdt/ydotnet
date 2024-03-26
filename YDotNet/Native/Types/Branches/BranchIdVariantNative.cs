using System.Runtime.InteropServices;

namespace YDotNet.Native.Types.Branches;

[StructLayout(LayoutKind.Explicit, Size = 8)]
internal readonly struct BranchIdVariantNative
{
    [field: FieldOffset(offset: 0)]
    public uint Clock { get; }

    [field: FieldOffset(offset: 0)]
    public nint NamePointer { get; }
}
