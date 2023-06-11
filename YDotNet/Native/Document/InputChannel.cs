using System.Runtime.InteropServices;
using YDotNet.Native.Cells;

namespace YDotNet.Native.Document;

internal static class InputChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_ydoc")]
    public static extern Input Doc(nint doc);
}
