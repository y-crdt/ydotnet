using System.Runtime.InteropServices;

namespace YDotNet.Native.Types.Branches;

[StructLayout(LayoutKind.Explicit, Size = 16)]
internal readonly struct BranchIdNative
{
    [field: FieldOffset(offset: 0)]
    public long ClientIdOrLength { get; }

    [field: FieldOffset(offset: 8)]
    public BranchIdVariantNative BranchIdVariant { get; }
}
