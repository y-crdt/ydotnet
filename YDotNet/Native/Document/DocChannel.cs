using System.Runtime.InteropServices;

namespace YDotNet.Native.Document;

internal static class DocChannel
{
    public delegate void ObserveAfterTransactionCallback(nint state, nint eventHandle);

    public delegate void ObserveClearCallback(nint state, nint docHandle);

    public delegate void ObserveSubdocsCallback(nint state, nint eventHandle);

    public delegate void ObserveUpdatesCallback(nint state, uint length, nint data);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_new_with_options")]
    public static extern nint NewWithOptions(DocOptionsNative options);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_clone")]
    public static extern nint Clone(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_destroy")]
    public static extern void Destroy(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_id")]
    public static extern ulong Id(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_guid")]
    public static extern nint Guid(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_collection_id")]
    public static extern nint CollectionId(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_should_load")]
    public static extern bool ShouldLoad(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_auto_load")]
    public static extern bool AutoLoad(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext")]
    public static extern nint Text(nint doc, nint name);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ymap")]
    public static extern nint Map(nint doc, nint name);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yarray")]
    public static extern nint Array(nint doc, nint name);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yxmlelem")]
    public static extern nint XmlElement(nint doc, nint name);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yxmltext")]
    public static extern nint XmlText(nint doc, nint name);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_read_transaction")]
    public static extern nint ReadTransaction(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_write_transaction")]
    public static extern nint WriteTransaction(nint doc, uint originLength, byte[]? origin);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_clear")]
    public static extern void Clear(nint doc);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_load")]
    public static extern void Load(nint doc, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_observe_clear")]
    public static extern uint ObserveClear(nint doc, nint state, ObserveClearCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_observe_updates_v1")]
    public static extern uint ObserveUpdatesV1(nint doc, nint state, ObserveUpdatesCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_observe_updates_v2")]
    public static extern uint ObserveUpdatesV2(nint doc, nint state, ObserveUpdatesCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_observe_after_transaction")]
    public static extern uint ObserveAfterTransaction(nint doc, nint state, ObserveAfterTransactionCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ydoc_observe_subdocs")]
    public static extern uint ObserveSubDocs(nint doc, nint state, ObserveSubdocsCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yunobserve")]
    public static extern uint Unobserve(uint subscriptionId);
}
