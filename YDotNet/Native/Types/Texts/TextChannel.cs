using System.Runtime.InteropServices;

namespace YDotNet.Native.Types.Texts;

internal static class TextChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext_insert")]
    public static extern void Insert(nint text, nint transaction, uint index, string value, nint attributes);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext_format")]
    public static extern void Format(nint text, nint transaction, uint index, uint length, nint attributes);

    // TODO [LSViana] Check if the transaction reference is really needed here.
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext_string")]
    public static extern nint String(nint text, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext_len")]
    public static extern uint Length(nint text, nint transaction);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext_chunks")]
    public static extern nint Chunks(nint text, nint transaction, out uint length);
}
