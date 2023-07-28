using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class EventChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yevent_keys_destroy")]
    public static extern void Destroy(nint keys, uint length);
}
