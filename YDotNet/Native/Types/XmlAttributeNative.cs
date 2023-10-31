using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct XmlAttributeNative
{
    public nint KeyHandle { get; }

    public nint ValueHandle { get; }

    public string Key()
    {
        return MemoryReader.ReadUtf8String(KeyHandle);
    }

    public string Value()
    {
        return MemoryReader.ReadUtf8String(ValueHandle);
    }
}
