using System.Runtime.InteropServices;

namespace YDotNet.Native.StickyIndex;

internal static class StickyIndexChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ysticky_index_destroy")]
    public static extern void Destroy(nint stickyIndex);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ysticky_index_from_index")]
    public static extern nint FromIndex(nint branch, nint transaction, uint index, sbyte associationType);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ysticky_index_assoc")]
    public static extern sbyte AssociationType(nint stickyIndex);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ysticky_index_read")]
    public static extern void Read(nint stickyIndex, nint transaction, out nint branch, out uint index);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ysticky_index_encode")]
    public static extern nint Encode(nint stickyIndex, out uint length);
}
