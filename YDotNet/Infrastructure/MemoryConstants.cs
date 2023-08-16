using System.Runtime.InteropServices;

namespace YDotNet.Infrastructure;

internal static class MemoryConstants
{
    public static int PointerSize { get; } = Marshal.SizeOf<nint>();
}
