using YDotNet.Document.State;

namespace YDotNet.Document.Events;

/// <summary>
///     An after transaction event passed to a callback subscribed to <see cref="Doc.ObserveAfterTransaction" />.
/// </summary>
public class AfterTransactionEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AfterTransactionEvent" /> class.
    /// </summary>
    /// <param name="beforeState">The initial value for <see cref="BeforeState" />.</param>
    /// <param name="afterState">The initial value for <see cref="AfterState" />.</param>
    /// <param name="deleteSet">The initial value for <see cref="DeleteSet" />.</param>
    public AfterTransactionEvent(StateVector beforeState, StateVector afterState, DeleteSet deleteSet)
    {
        BeforeState = beforeState;
        AfterState = afterState;
        DeleteSet = deleteSet;
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
