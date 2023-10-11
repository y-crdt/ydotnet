using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.StickyIndexes;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.StickyIndex;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays;

/// <summary>
///     A shared data type that represents an array.
/// </summary>
public class Array : Branch
{
    private readonly EventSubscriptions subscriptions = new EventSubscriptions();

    internal Array(nint handle)
        : base(handle)
    {
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
    public void InsertRange(Transaction transaction, uint index, params Input[] inputs)
    {
        using var unsafeInputs = MemoryWriter.WriteStructArray(inputs.Select(x => x.InputNative).ToArray());

        ArrayChannel.InsertRange(Handle, transaction.Handle, index, unsafeInputs.Handle, (uint)inputs.Length);
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

        return handle != nint.Zero ? new Output(handle, null) : null;
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
    /// <returns>The <see cref="ArrayIterator" /> instance.</returns>
    public ArrayIterator Iterate(Transaction transaction)
    {
        var handle = ArrayChannel.Iterator(Handle, transaction.Handle);

        return new ArrayIterator(handle.Checked());
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="Array" /> instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable Observe(Action<ArrayEvent> action)
    {
        ArrayChannel.ObserveCallback callback = (_, eventHandle) => action(new ArrayEvent(eventHandle));

        var subscriptionId = ArrayChannel.Observe(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            ArrayChannel.Unobserve(Handle, subscriptionId);
        });
    }

    /// <summary>
    ///     Retrieves a <see cref="StickyIndex" /> corresponding to a given human-readable <see cref="index" /> pointing into
    ///     the <see cref="Branch" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The numeric index to place the <see cref="StickyIndex" />.</param>
    /// <param name="associationType">The type of the <see cref="StickyIndex" />.</param>
    /// <returns>The <see cref="StickyIndex" /> in the <see cref="index" /> with the given <see cref="associationType" />.</returns>
    public StickyIndex? StickyIndex(Transaction transaction, uint index, StickyAssociationType associationType)
    {
        var handle = StickyIndexChannel.FromIndex(Handle, transaction.Handle, index, (sbyte)associationType);

        return handle != nint.Zero ? new StickyIndex(handle) : null;
    }
}
