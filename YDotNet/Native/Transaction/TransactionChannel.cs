using System.Runtime.InteropServices;

namespace YDotNet.Native.Transaction;

internal static class TransactionChannel
{
    [DllImport(
        ChannelSettings.NativeLib, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ytransaction_commit")]
    public static extern nint Commit(nint transaction);
}
