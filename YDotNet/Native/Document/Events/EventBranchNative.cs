using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential, Size = Size)]
internal struct EventBranchNative
{
    public const int Size = 8 + OutputNative.Size;

    public uint Tag { get; }

    public nint ValueHandle(nint baseHandle)
    {
        return baseHandle + MemoryConstants.PointerSize;
    }
}
