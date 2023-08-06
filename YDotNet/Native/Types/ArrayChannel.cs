using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class ArrayChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_len")]
    public static extern uint Length(nint array);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_insert_range")]
    public static extern void InsertRange(nint array, nint transaction, uint index, nint items, uint itemsLength);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_remove_range")]
    public static extern void RemoveRange(nint array, nint transaction, uint index, uint length);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_get")]
    public static extern nint Get(nint array, nint transaction, uint index);
}
