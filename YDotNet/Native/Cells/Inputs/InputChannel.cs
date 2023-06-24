using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Inputs;

internal static class InputChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_ydoc")]
    public static extern InputNative Doc(nint doc);
}
