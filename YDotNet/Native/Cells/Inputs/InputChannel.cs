using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Inputs;

internal static class InputChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_ydoc")]
    public static extern InputNative Doc(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_string")]
    public static extern InputNative String(nint value);
}
