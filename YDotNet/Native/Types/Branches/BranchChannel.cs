using System.Runtime.InteropServices;

namespace YDotNet.Native.Types.Branches;

internal static class BranchChannel
{
    public delegate void ObserveCallback(nint state, uint length, nint eventsHandle);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yobserve_deep")]
    public static extern nint ObserveDeep(nint type, nint state, ObserveCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytype_kind")]
    public static extern byte Kind(nint branch);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ybranch_read_transaction")]
    public static extern nint ReadTransaction(nint branch);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ybranch_write_transaction")]
    public static extern nint WriteTransaction(nint branch);
}
