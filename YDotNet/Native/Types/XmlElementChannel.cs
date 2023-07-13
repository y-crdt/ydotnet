using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlElementChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlelem_tag")]
    public static extern string Tag(nint handle);
}
