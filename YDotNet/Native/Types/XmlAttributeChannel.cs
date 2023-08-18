using System.Runtime.InteropServices;

namespace YDotNet.Native.Types;

internal static class XmlAttributeChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlattr_iter_next")]
    public static extern nint IteratorNext(nint handle);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlattr_iter_destroy")]
    public static extern nint IteratorDestroy(nint handle);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yxmlattr_destroy")]
    public static extern nint Destroy(nint handle);
}
