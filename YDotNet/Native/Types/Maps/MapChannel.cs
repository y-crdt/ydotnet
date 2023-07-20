using System.Runtime.InteropServices;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Native.Types.Maps;

internal static class MapChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_insert")]
    public static extern void Insert(nint map, nint transaction, string key, InputNative inputNative);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_get")]
    public static extern nint Get(nint map, nint transaction, string key);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_len")]
    public static extern uint Length(nint map, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_remove")]
    public static extern byte Remove(nint map, nint transaction, string key);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_remove_all")]
    public static extern void RemoveAll(nint map, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_iter")]
    public static extern nint Iterator(nint map, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_iter_next")]
    public static extern nint IteratorNext(nint mapIterator);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_iter_destroy")]
    public static extern nint IteratorDestroy(nint mapIterator);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_entry_destroy")]
    public static extern nint EntryDestroy(nint mapEntry);
}
