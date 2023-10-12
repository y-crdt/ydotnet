using YDotNet.Document.State;
using YDotNet.Native.Document.Events;

namespace YDotNet.Document.Events;

/// <summary>
///     An after transaction event passed to a callback subscribed to <see cref="Doc.ObserveAfterTransaction" />.
/// </summary>
public class AfterTransactionEvent
{
    internal AfterTransactionEvent(AfterTransactionEventNative native)
    {
        BeforeState = new StateVector(native.BeforeState);
        AfterState = new StateVector(native.AfterState);
        DeleteSet = new DeleteSet(native.DeleteSet);
    }

    /// <summary>
    ///     Gets descriptor of a document state at the moment of creating the transaction.
    /// </summary>
    public StateVector BeforeState { get; }

    /// <summary>
    ///     Gets descriptor of a document state at the moment of committing the transaction.
    /// </summary>
    public StateVector AfterState { get; }

    /// <summary>
    ///     Gets information about all items deleted within the scope of a transaction.
    /// </summary>
    public DeleteSet DeleteSet { get; }
}
