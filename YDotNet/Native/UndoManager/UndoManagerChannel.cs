using System.Runtime.InteropServices;
using YDotNet.Native.UndoManager.Events;

namespace YDotNet.Native.UndoManager;

internal static class UndoManagerChannel
{
    public delegate void ObserveAddedCallback(nint state, UndoEventNative undoEvent);

    [DllImport(ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager")]
    public static extern nint NewWithOptions(nint doc, nint branch, nint options);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager_destroy")]
    public static extern void Destroy(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_observe_added")]
    public static extern uint ObserveAdded(nint undoManager, nint state, ObserveAddedCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_unobserve_added")]
    public static extern uint UnobserveAdded(nint undoManager, uint subscriptionId);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager_undo")]
    public static extern byte Undo(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager_redo")]
    public static extern byte Redo(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "yundo_manager_can_undo")]
    public static extern byte CanUndo(nint undoManager);
}
