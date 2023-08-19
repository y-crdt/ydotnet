using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlElementChannel
{
    public delegate void ObserveCallback(nint state, nint eventHandle);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_tag")]
    public static extern nint Tag(nint handle);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_string")]
    public static extern nint String(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_insert_attr")]
    public static extern void InsertAttribute(nint handle, nint transaction, nint name, nint value);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_remove_attr")]
    public static extern void RemoveAttribute(nint handle, nint transaction, nint name);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_get_attr")]
    public static extern nint GetAttribute(nint handle, nint transaction, nint name);

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
    public static extern nint InsertElement(nint handle, nint transaction, uint index, nint name);

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

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_tree_walker")]
    public static extern nint TreeWalker(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yxmlelem_tree_walker_next")]
    public static extern nint TreeWalkerNext(nint handle);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yxmlelem_tree_walker_destroy")]
    public static extern void TreeWalkerDestroy(nint handle);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_observe")]
    public static extern uint Observe(nint handle, nint state, ObserveCallback callback);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_event_target")]
    public static extern nint ObserveEventTarget(nint arrayEvent);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_event_path")]
    public static extern nint ObserveEventPath(nint arrayEvent, out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_event_delta")]
    public static extern nint ObserveEventDelta(nint arrayEvent, out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_event_keys")]
    public static extern nint ObserveEventKeys(nint arrayEvent, out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_unobserve")]
    public static extern void Unobserve(nint handle, uint subscriptionId);
}
