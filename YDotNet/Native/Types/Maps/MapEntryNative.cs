using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Types.Maps;

[StructLayout(LayoutKind.Sequential, Size = Size)]
internal readonly struct MapEntryNative
{
    internal const int Size = 8 + OutputNative.Size;

    internal nint KeyHandle { get; }

    public nint ValueHandle(nint baseHandle)
    {
        return baseHandle + MemoryConstants.PointerSize;
    }

    public string Key()
    {
        return MemoryReader.ReadUtf8String(KeyHandle);
    }
}
