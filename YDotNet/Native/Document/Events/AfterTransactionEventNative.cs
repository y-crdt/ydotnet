using System.Runtime.InteropServices;
using YDotNet.Document.Events;
using YDotNet.Native.Document.State;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
public class AfterTransactionEventNative
{
    public StateVectorNative BeforeState { get; init; }

    public StateVectorNative AfterState { get; init; }

    public DeleteSetNative DeleteSet { get; set; }

    public AfterTransactionEvent ToAfterTransactionEvent()
    {
        return new AfterTransactionEvent(
            BeforeState.ToStateVector(),
            AfterState.ToStateVector(),
            DeleteSet.ToDeleteSet());
    }
}
