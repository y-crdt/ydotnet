using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells;

internal static class InputChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_ydoc")]
    public static extern Input Doc(nint doc);
}
