using System.Runtime.InteropServices;

namespace YDotNet.Native.UndoManager;

internal static class UndoManagerChannel
{
    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager")]
    public static extern nint NewWithOptions(nint doc, nint branch, nint options);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager_destroy")]
    public static extern void Destroy(nint undoManager);
}
