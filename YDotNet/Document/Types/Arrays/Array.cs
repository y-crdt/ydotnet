using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays;

/// <summary>
///     A shared data type that represents an array.
/// </summary>
public class Array : Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Array" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Array(nint handle)
        : base(handle)
    {
        // Nothing here.
    }

    /// <summary>
    ///     Gets the number of elements stored within current instance of <see cref="Types.Array" />.
    /// </summary>
    public uint Length => ArrayChannel.Length(Handle);

    /// <summary>
    ///     Inserts a range of <see cref="inputs" /> into the current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The starting index to insert the items.</param>
    /// <param name="inputs">The items to be inserted.</param>
    public void InsertRange(Transaction transaction, uint index, IEnumerable<Input> inputs)
    {
        var inputsArray = inputs.Select(x => x.InputNative).ToArray();
        var inputsPointer = MemoryWriter.WriteStructArray(inputsArray);

        ArrayChannel.InsertRange(Handle, transaction.Handle, index, inputsPointer, (uint) inputsArray.Length);
    }

    /// <summary>
    ///     Removes a range of items from the current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The starting index to remove the items.</param>
    /// <param name="length">The amount of items to remove.</param>
    public void RemoveRange(Transaction transaction, uint index, uint length)
    {
        ArrayChannel.RemoveRange(Handle, transaction.Handle, index, length);
    }

    /// <summary>
    ///     Gets the <see cref="Output" /> value at the given <see cref="index" /> or
    ///     <c>null</c> if <see cref="index" /> is outside the bounds.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index to get the item.</param>
    /// <returns>
    ///     The <see cref="Output" /> value at the given <see cref="index" /> or <c>null</c> if <see cref="index" /> is
    ///     outside the bounds.
    /// </returns>
    public Output? Get(Transaction transaction, uint index)
    {
        var handle = ArrayChannel.Get(Handle, transaction.Handle, index);

        return handle == nint.Zero ? null : new Output(handle);
    }

    /// <summary>
    ///     Moves the element at <see cref="sourceIndex" /> to the <see cref="targetIndex" />.
    /// </summary>
    /// <remarks>
    ///     Both indexes must be lower than the <see cref="Length" />.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="sourceIndex">The index of the item that will be moved.</param>
    /// <param name="targetIndex">The index to which the item will be moved to.</param>
    public void Move(Transaction transaction, uint sourceIndex, uint targetIndex)
    {
        ArrayChannel.Move(Handle, transaction.Handle, sourceIndex, targetIndex);
    }

    /// <summary>
    ///     Returns a <see cref="ArrayIterator" />, which can be used to traverse
    ///     over all values of this <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The <see cref="ArrayIterator" /> instance or <c>null</c> if failed.</returns>
    public ArrayIterator? Iterate(Transaction transaction)
    {
        return ReferenceAccessor.Access(new ArrayIterator(ArrayChannel.Iterator(Handle, transaction.Handle)));
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="Array" /> instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription Observe(Action<ArrayEvent> action)
    {
        var subscriptionId = ArrayChannel.Observe(
            Handle,
            nint.Zero,
            (state, eventHandle) => action(new ArrayEvent(eventHandle)));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed within <see cref="Array" /> scope.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void Unobserve(EventSubscription subscription)
    {
        ArrayChannel.Unobserve(Handle, subscription.Id);
    }
}
