using System.Runtime.InteropServices;

namespace YDotNet.Native.Types.Texts;

internal static class TextChannel
{
    public delegate void ObserveCallback(nint state, nint textEvent);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_insert")]
    public static extern void Insert(nint text, nint transaction, uint index, nint value, nint attributes);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_insert_embed")]
    public static extern void InsertEmbed(nint text, nint transaction, uint index, nint content, nint attributes);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_remove_range")]
    public static extern void RemoveRange(nint text, nint transaction, uint index, uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_format")]
    public static extern void Format(nint text, nint transaction, uint index, uint length, nint attributes);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_string")]
    public static extern nint String(nint text, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_len")]
    public static extern uint Length(nint text, nint transaction);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_chunks")]
    public static extern nint Chunks(nint text, nint transaction, out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_observe")]
    public static extern nint Observe(nint text, nint state, ObserveCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_event_path")]
    public static extern nint ObserveEventPath(nint textEvent, out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_event_delta")]
    public static extern nint ObserveEventDelta(nint textEvent, out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytext_event_target")]
    public static extern nint ObserveEventTarget(nint textEvent);
}
