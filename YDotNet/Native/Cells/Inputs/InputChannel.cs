using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells.Inputs;

internal static class InputChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_ydoc")]
    public static extern InputNative Doc(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_string")]
    public static extern InputNative String(nint value);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_bool")]
    public static extern InputNative Boolean(bool value);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_float")]
    public static extern InputNative Double(double value);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_long")]
    public static extern InputNative Long(long value);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_binary")]
    public static extern InputNative Bytes(byte[] value, uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_json_array")]
    public static extern InputNative Collection(nint values, uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_json_map")]
    public static extern InputNative Object(nint keys, nint values, uint length);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_null")]
    public static extern InputNative Null();

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yinput_undefined")]
    public static extern InputNative Undefined();
}
