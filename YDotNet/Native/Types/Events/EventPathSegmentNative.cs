using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Explicit)]
internal readonly struct EventPathSegmentNative
{
    [field: FieldOffset(0)]
    public byte Tag { get; }

    [field: FieldOffset(8)]
    public nint KeyOrIndex { get; }

    public string Key()
    {
        return MemoryReader.ReadUtf8String(KeyOrIndex);
    }
}
