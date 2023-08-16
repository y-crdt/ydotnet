using System.Runtime.InteropServices;

namespace YDotNet.Infrastructure;

internal static class MemoryConstants
{
    internal static readonly int PointerSize = Marshal.SizeOf<nint>();
}
