using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Outputs;

internal static class OutputChannel
{
    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_ydoc")]
    public static extern nint Doc(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_string")]
    public static extern nint String(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_bool")]
    public static extern nint Boolean(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_float")]
    public static extern nint Double(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_long")]
    public static extern nint Long(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_binary")]
    public static extern nint Bytes(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_json_array")]
    public static extern nint Collection(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_json_map")]
    public static extern nint Object(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_yarray")]
    public static extern nint Array(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_ymap")]
    public static extern nint Map(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_ytext")]
    public static extern nint Text(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_yxmlelem")]
    public static extern nint XmlElement(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_read_yxmltext")]
    public static extern nint XmlText(nint output);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "youtput_destroy")]
    public static extern void Destroy(nint output);
}
