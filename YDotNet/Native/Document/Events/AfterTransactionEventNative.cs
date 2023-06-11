using System.Runtime.InteropServices;
using YDotNet.Document.Events;
using YDotNet.Native.Document.State;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct AfterTransactionEventNative
{
    public StateVectorNative BeforeState { get; }

    public StateVectorNative AfterState { get; }

    public DeleteSetNative DeleteSet { get; }

    public AfterTransactionEvent ToAfterTransactionEvent()
    {
        return new AfterTransactionEvent(
            BeforeState.ToStateVector(),
            AfterState.ToStateVector(),
            DeleteSet.ToDeleteSet());
    }
}
