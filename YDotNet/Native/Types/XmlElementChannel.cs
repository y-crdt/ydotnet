using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlElementChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_tag")]
    public static extern nint Tag(nint handle);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_string")]
    public static extern nint String(nint handle);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_insert_attr")]
    public static extern void InsertAttribute(nint handle, nint transaction, string name, string value);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_remove_attr")]
    public static extern void RemoveAttribute(nint handle, nint transaction, string name);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_get_attr")]
    public static extern nint GetAttribute(nint handle, nint transaction, string name);
}
