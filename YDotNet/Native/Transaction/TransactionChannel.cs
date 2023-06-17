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
}
