using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Maps;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MapEntryNative
{
    internal nint KeyHandle { get; }

    public nint ValueHandle { get; }

    public string Key()
    {
        return MemoryReader.ReadUtf8String(KeyHandle);
    }
}
