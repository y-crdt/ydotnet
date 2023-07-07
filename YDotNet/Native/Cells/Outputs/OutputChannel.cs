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

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_bool")]
    public static extern nint Boolean(nint output);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_float")]
    public static extern nint Double(nint output);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_long")]
    public static extern nint Long(nint output);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_binary")]
    public static extern nint Bytes(nint output);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_json_array")]
    public static extern nint Collection(nint output);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "youtput_read_json_null")]
    public static extern byte Null(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_json_undefined")]
    public static extern byte Undefined(nint output);
}
