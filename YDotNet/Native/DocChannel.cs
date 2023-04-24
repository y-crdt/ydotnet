using System.Runtime.InteropServices;

namespace YDotNet.Native;

internal static class DocChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_new")]
    public static extern nint New();

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_destroy")]
    public static extern void Destroy(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_id")]
    public static extern ulong Id(nint doc);
}
