using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class BinaryChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ybinary_destroy")]
    public static extern void Destroy(nint handle, uint length);
}
