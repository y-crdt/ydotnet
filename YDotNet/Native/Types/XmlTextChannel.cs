using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlTextChannel
{
    public delegate void ObserveCallback(nint state, nint eventHandle);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_len")]
    public static extern uint Length(nint handle, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_insert")]
    public static extern uint Insert(nint handle, nint transaction, uint index, nint value, nint attributes);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_insert_embed")]
    public static extern void InsertEmbed(nint handle, nint transaction, uint index, nint content, nint attributes);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_insert_attr")]
    public static extern void InsertAttribute(nint handle, nint transaction, nint name, nint value);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_get_attr")]
    public static extern nint GetAttribute(nint handle, nint transaction, nint name);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_remove_attr")]
    public static extern nint RemoveAttribute(nint handle, nint transaction, nint name);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_attr_iter")]
    public static extern nint AttributeIterator(nint handle, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_string")]
    public static extern nint String(nint handle, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_remove_range")]
    public static extern void RemoveRange(nint handle, nint transaction, uint index, uint length);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_format")]
    public static extern void Format(nint handle, nint transaction, uint index, uint length, nint attributes);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_observe")]
    public static extern uint Observe(nint handle, nint state, ObserveCallback callback);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_event_target")]
    public static extern nint ObserveEventTarget(nint eventHandle);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_event_delta")]
    public static extern nint ObserveEventDelta(nint eventHandle, out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_event_keys")]
    public static extern nint ObserveEventKeys(nint eventHandle, out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmltext_unobserve")]
    public static extern void Unobserve(nint handle, uint subscriptionId);
}
