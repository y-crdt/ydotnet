using System.Runtime.InteropServices;
using YDotNet.Document.Events;
using YDotNet.Native.Document.State;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Explicit)]
internal struct AfterTransactionEventNative
{
    [field: FieldOffset(0)]
    public StateVectorNative BeforeState { get; }

    [field: FieldOffset(24)]
    public StateVectorNative AfterState { get; }

    [field: FieldOffset(48)]
    public DeleteSetNative DeleteSet { get; }

    public AfterTransactionEvent ToAfterTransactionEvent()
    {
        return new AfterTransactionEvent(
            BeforeState.ToStateVector(),
            AfterState.ToStateVector(),
            DeleteSet.ToDeleteSet());
    }
}
