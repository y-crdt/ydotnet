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

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_attr_iter")]
    public static extern nint AttributeIterator(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_child_len")]
    public static extern uint ChildLength(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_insert_text")]
    public static extern nint InsertText(nint handle, nint transaction, uint index);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_insert_elem")]
    public static extern nint InsertElement(nint handle, nint transaction, uint index, string name);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_remove_range")]
    public static extern nint RemoveRange(nint handle, nint transaction, uint index, uint length);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_get")]
    public static extern nint Get(nint handle, nint transaction, uint index);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_parent")]
    public static extern nint Parent(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_first_child")]
    public static extern nint FirstChild(nint handle, nint transaction);
}
