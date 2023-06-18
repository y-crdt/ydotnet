using System.Runtime.InteropServices;
using YDotNet.Native.Cells;

namespace YDotNet.Native.Types;

internal static class MapChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_insert")]
    public static extern void Insert(nint map, nint transaction, string key, Input input);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_remove")]
    public static extern bool Remove(nint map, nint transaction, string key);
}
