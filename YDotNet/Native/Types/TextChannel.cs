using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class TextChannel
{
    // TODO [LSViana] Add class for `attributes` parameter.
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytext_insert")]
    public static extern void Insert(nint text, nint transaction, uint index, string value, nint attributes);
}
