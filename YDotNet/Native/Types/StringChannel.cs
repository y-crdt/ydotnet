using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class StringChannel
{
    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ystring_destroy")]
    public static extern void Destroy(nint handle);
}
