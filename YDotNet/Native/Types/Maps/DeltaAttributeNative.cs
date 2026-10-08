using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Types.Maps;

[StructLayout(LayoutKind.Sequential, Size = Size)]
/// <summary>
///     Maps to <c>YDeltaAttr</c>, whose value is stored inline, unlike <c>YMapEntry</c>.
/// </summary>
internal readonly struct DeltaAttributeNative
{
    private const int Size = 8 + OutputNative.Size;

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
