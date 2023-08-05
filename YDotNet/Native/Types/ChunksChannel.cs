using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class ChunksChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ychunks_destroy")]
    public static extern nint Destroy(nint chunks, uint length);
}
