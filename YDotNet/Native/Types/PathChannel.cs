using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class PathChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ypath_destroy")]
    public static extern uint Destroy(nint paths, uint length);
}
