using System.Runtime.InteropServices;

namespace YDotNet.Native.UndoManager;

internal static class UndoManagerChannel
{
    public delegate void ObserveAddedCallback(nint state, nint eventHandle);

    public delegate void ObservePoppedCallback(nint state, nint eventHandle);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager")]
    public static extern nint NewWithOptions(nint doc, nint branch, nint options);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_destroy")]
    public static extern void Destroy(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_observe_added")]
    public static extern uint ObserveAdded(nint undoManager, nint state, ObserveAddedCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_observe_popped")]
    public static extern uint ObservePopped(nint undoManager, nint state, ObservePoppedCallback callback);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_undo")]
    public static extern byte Undo(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_redo")]
    public static extern byte Redo(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_can_undo")]
    public static extern byte CanUndo(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_can_redo")]
    public static extern byte CanRedo(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_clear")]
    public static extern byte Clear(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_stop")]
    public static extern void Stop(nint undoManager);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_add_scope")]
    public static extern void AddScope(nint undoManager, nint branch);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_add_origin")]
    public static extern void AddOrigin(nint undoManager, uint originLength, byte[] origin);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "yundo_manager_remove_origin")]
    public static extern void RemoveOrigin(nint undoManager, uint originLength, byte[] origin);
}
