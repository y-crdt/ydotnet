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

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_move")]
    public static extern void Move(nint array, nint transaction, uint sourceIndex, uint targetIndex);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_iter")]
    public static extern nint Iterator(nint array, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_iter_destroy")]
    public static extern nint IteratorDestroy(nint arrayIterator);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yarray_iter_next")]
    public static extern nint IteratorNext(nint arrayIterator);
}
