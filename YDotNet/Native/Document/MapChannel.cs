using System.Runtime.InteropServices;
using YDotNet.Native.Cells;

namespace YDotNet.Native.Document;

internal static class MapChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ymap_insert")]
    public static extern void Insert(nint map, nint transaction, string key, Input input);
}
