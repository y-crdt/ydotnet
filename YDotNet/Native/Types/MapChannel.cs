using System.Runtime.InteropServices;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Native.Types;

internal static class MapChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_insert")]
    public static extern void Insert(nint map, nint transaction, string key, InputNative inputNative);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_remove")]
    public static extern bool Remove(nint map, nint transaction, string key);
}
