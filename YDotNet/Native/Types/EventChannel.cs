using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class EventChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yevent_keys_destroy")]
    public static extern void KeysDestroy(nint keys, uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yevent_delta_destroy")]
    public static extern void DeltaDestroy(nint deltas, uint length);
}
