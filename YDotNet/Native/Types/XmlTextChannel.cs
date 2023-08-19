using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlTextChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_len")]
    public static extern uint Length(nint handle, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_insert")]
    public static extern uint Insert(nint handle, nint transaction, uint index, nint value, nint attributes);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_insert_embed")]
    public static extern void InsertEmbed(nint handle, nint transaction, uint index, nint content, nint attributes);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_string")]
    public static extern nint String(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_remove_range")]
    public static extern void RemoveRange(nint handle, nint transaction, uint index, uint length);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_format")]
    public static extern void Format(nint handle, nint transaction, uint index, uint length, nint attributes);
}
