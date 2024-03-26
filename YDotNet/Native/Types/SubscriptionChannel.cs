using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class SubscriptionChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yunobserve")]
    public static extern void Unobserve(nint subscription);
}
