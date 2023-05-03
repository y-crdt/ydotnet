using System.Runtime.InteropServices;

namespace YDotNet.Native.Document;

internal static class DocChannel
{
    public delegate void ObserveUpdatesCallback(nint state, uint length, nint data);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_new")]
    public static extern nint New();

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_new_with_options")]
    public static extern nint NewWithOptions(DocOptionsNative options);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_destroy")]
    public static extern void Destroy(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_id")]
    public static extern ulong Id(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_guid")]
    public static extern string Guid(nint doc);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_collection_id")]
    public static extern string CollectionId(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_should_load")]
    public static extern bool ShouldLoad(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_auto_load")]
    public static extern bool AutoLoad(nint doc);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext")]
    public static extern nint Text(nint doc, string name);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_write_transaction")]
    public static extern nint WriteTransaction(nint doc, uint originLength, nint origin);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_observe_updates_v1")]
    public static extern uint ObserveUpdatesV1(nint doc, nint state, ObserveUpdatesCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_unobserve_updates_v1")]
    public static extern uint UnobserveUpdatesV1(nint doc, uint subscriptionId);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ydoc_observe_updates_v2")]
    public static extern uint ObserveUpdatesV2(nint doc, nint state, ObserveUpdatesCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_unobserve_updates_v2")]
    public static extern uint UnobserveUpdatesV2(nint doc, uint subscriptionId);
}
