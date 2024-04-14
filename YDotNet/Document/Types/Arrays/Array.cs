using System.Reflection.Metadata;
using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.StickyIndexes;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.StickyIndex;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays;

/// <summary>
///     A shared data type that represents an array.
/// </summary>
public class Array : Branch
{
    private readonly EventSubscriberFromId<ArrayEvent> onObserve;

    internal Array(nint handle, Doc doc, bool isDeleted)
        : base(handle, doc, isDeleted)
    {
        onObserve = new EventSubscriberFromId<ArrayEvent>(
            doc.EventManager,
            this,
            (array, action) =>
            {
                ArrayChannel.ObserveCallback callback = (_, eventHandle) =>
                    action(new ArrayEvent(eventHandle, Doc));

                return (ArrayChannel.Observe(array, nint.Zero, callback), callback);
            },
            SubscriptionChannel.Unobserve);
    }

    /// <summary>
    ///     Gets the number of elements stored within current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    /// The length.
    /// </returns>
    public uint Length(Transaction transaction)
    {
        var handle = BranchId.GetHandle(transaction);

        return ArrayChannel.Length(handle);
    }

    /// <summary>
    ///     Inserts a range of <paramref name="inputs" /> into the current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The starting index to insert the items.</param>
    /// <param name="inputs">The items to be inserted.</param>
    public void InsertRange(Transaction transaction, uint index, params Input[] inputs)
    {
        var handle = BranchId.GetHandle(transaction);

        using var unsafeInputs = MemoryWriter.WriteStructArray(inputs.Select(x => x.InputNative).ToArray());

        ArrayChannel.InsertRange(handle, transaction.Handle, index, unsafeInputs.Handle, (uint)inputs.Length);
    }

    /// <summary>
    ///     Removes a range of items from the current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The starting index to remove the items.</param>
    /// <param name="length">The amount of items to remove.</param>
    public void RemoveRange(Transaction transaction, uint index, uint length)
    {
        var handle = BranchId.GetHandle(transaction);

        ArrayChannel.RemoveRange(handle, transaction.Handle, index, length);
    }

    /// <summary>
    ///     Gets the <see cref="Output" /> value at the given <paramref name="index" /> or
    ///     <c>null</c> if <paramref name="index" /> is outside the bounds.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index to get the item.</param>
    /// <returns>
    ///     The <see cref="Output" /> value at the given <paramref name="index" /> or <c>null</c> if <paramref name="index" />
    ///     is
    ///     outside the bounds.
    /// </returns>
    public Output? Get(Transaction transaction, uint index)
    {
        var handle = BranchId.GetHandle(transaction);

        var outputHandle = ArrayChannel.Get(handle, transaction.Handle, index);

        return outputHandle != nint.Zero ? Output.CreateAndRelease(outputHandle, Doc) : null;
    }

    /// <summary>
    ///     Moves the element at <paramref name="sourceIndex" /> to the <paramref name="targetIndex" />.
    /// </summary>
    /// <remarks>
    ///     Both indexes must be lower than the <see cref="Length" />.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="sourceIndex">The index of the item that will be moved.</param>
    /// <param name="targetIndex">The index to which the item will be moved to.</param>
    public void Move(Transaction transaction, uint sourceIndex, uint targetIndex)
    {
        var handle = BranchId.GetHandle(transaction);

        ArrayChannel.Move(handle, transaction.Handle, sourceIndex, targetIndex);
    }

    /// <summary>
    ///     Returns a <see cref="ArrayIterator" />, which can be used to traverse
    ///     over all values of this <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The <see cref="ArrayIterator" /> instance.</returns>
    public ArrayIterator Iterate(Transaction transaction)
    {
        var handle = BranchId.GetHandle(transaction);

        var iteratorHandle = ArrayChannel.Iterator(handle, transaction.Handle);

        return new ArrayIterator(iteratorHandle.Checked(), Doc);
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
        return onObserve.Subscribe(action);
    }

    /// <summary>
    ///     Retrieves a <see cref="StickyIndex" /> corresponding to a given human-readable <paramref name="index" /> pointing
    ///     into
    ///     the <see cref="Branch" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The numeric index to place the <see cref="StickyIndex" />.</param>
    /// <param name="associationType">The type of the <see cref="StickyIndex" />.</param>
    /// <returns>
    ///     The <see cref="StickyIndex" /> in the <paramref name="index" /> with the given
    ///     <paramref name="associationType" />.
    /// </returns>
    public StickyIndex? StickyIndex(Transaction transaction, uint index, StickyAssociationType associationType)
    {
        var handle = BranchId.GetHandle(transaction);

        var indexHandle = StickyIndexChannel.FromIndex(handle, transaction.Handle, index, (sbyte)associationType);

        return indexHandle != nint.Zero ? new StickyIndex(indexHandle) : null;
    }
}
