using System.Runtime.InteropServices;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Types.Maps;

[StructLayout(LayoutKind.Sequential, Size = OutputNative.Size + 8)]
internal struct MapEntryNative
{
    public nint Field { get; }
}
