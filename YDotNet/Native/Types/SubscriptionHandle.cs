using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using YDotNet.Native.Document;

namespace YDotNet.Native.Types;

internal sealed class SubscriptionHandle : UnmanagedResourceHandle
{
    public static void Unobserve(SubscriptionHandle handle)
    {
        handle.Dispose();
    }

    protected override bool ReleaseHandle()
    {
        Native.Unobserve(handle);
        return true;
    }

    private static class Native
    {
        [DllImport(
            ChannelSettings.NativeLib,
            CallingConvention = CallingConvention.Cdecl,
            EntryPoint = "yunobserve")]
        public static extern void Unobserve(nint subscription);
    }
}
