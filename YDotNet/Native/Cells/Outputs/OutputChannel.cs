using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Outputs;

internal static class OutputChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_ydoc")]
    public static extern nint Doc(nint output);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_string")]
    public static extern string? String(nint output);
}
