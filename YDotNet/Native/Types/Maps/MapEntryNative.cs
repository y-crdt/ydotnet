using System.Runtime.InteropServices;

namespace YDotNet.Native.Types.Maps;

// TODO [LSViana] Make it easier to connect OutputNative size (16) with string pointer size (8).
[StructLayout(LayoutKind.Sequential, Size = 24)]
internal struct MapEntryNative
{
    public nint Field { get; }

    // TODO [LSViana] Check if this field is needed or if it can be removed completely.
    // public OutputNative OutputNative { get; }
}
