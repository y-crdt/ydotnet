using System.Runtime.InteropServices;
using YDotNet.Native.Document.State;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct AfterTransactionEventNative
{
    public StateVectorNative BeforeState { get; }

    public StateVectorNative AfterState { get; }

    public DeleteSetNative DeleteSet { get; }
}
