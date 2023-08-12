using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxml_next_sibling")]
    public static extern nint NextSibling(nint handle, nint transaction);
}
