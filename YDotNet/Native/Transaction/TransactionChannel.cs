using System.Runtime.InteropServices;

namespace YDotNet.Native.Transaction;

internal static class TransactionChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_commit")]
    public static extern nint Commit(nint transaction);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_subdocs")]
    public static extern nint SubDocs(nint transaction, out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_writeable")]
    public static extern byte Writeable(nint transaction);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytransaction_state_vector_v1")]
    public static extern nint StateVectorV1(nint transaction, out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytransaction_state_diff_v1")]
    public static extern nint StateDiffV1(
        nint transaction,
        byte[] stateVector,
        uint stateVectorLength,
        out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytransaction_state_diff_v2")]
    public static extern nint StateDiffV2(
        nint transaction,
        byte[] stateVector,
        uint stateVectorLength,
        out uint length);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_apply")]
    public static extern byte ApplyV1(
        nint transaction,
        byte[] stateDiff,
        uint stateDiffLength);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_apply_v2")]
    public static extern byte ApplyV2(
        nint transaction,
        byte[] stateDiff,
        uint stateDiffLength);

    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_snapshot")]
    public static extern nint Snapshot(nint transaction, out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytransaction_encode_state_from_snapshot_v1")]
    public static extern nint EncodeStateFromSnapshotV1(
        nint transaction,
        byte[] snapshot,
        uint snapshotLength,
        out uint length);

    [DllImport(
        ChannelSettings.NativeLib,
        CallingConvention = CallingConvention.Cdecl,
        EntryPoint = "ytransaction_encode_state_from_snapshot_v2")]
    public static extern nint EncodeStateFromSnapshotV2(
        nint transaction,
        byte[] snapshot,
        uint snapshotLength,
        out uint length);
}
