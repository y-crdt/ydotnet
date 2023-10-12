using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     The generic type that can be used to refer to all shared data type instances.
/// </summary>
public abstract class Branch : TypeBase
{
    private readonly EventSubscriptions subscriptions = new();
    private readonly Doc doc;

    internal Branch(nint handle, Doc doc)
    {
        Doc = doc;

        Handle = handle;
    }

    internal Doc Doc { get; }

    internal nint Handle { get; }

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="Branch" /> instance
    ///     and all nested types.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveDeep(Action<IEnumerable<EventBranch>> action)
    {
        BranchChannel.ObserveCallback callback = (_, length, eventsHandle) =>
        {
            var events = MemoryReader.ReadIntPtrArray(eventsHandle, length, size: 24).Select(x => new EventBranch(x, doc)).ToArray();

            action(events);
        };

        var subscriptionId = BranchChannel.ObserveDeep(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            BranchChannel.UnobserveDeep(Handle, subscriptionId);
        });
    }

    /// <summary>
    ///     Starts a new read-write <see cref="Transaction" /> on this <see cref="Branch" /> instance.
    /// </summary>
    /// <returns>The <see cref="Transaction" /> to perform operations in the document.</returns>
    /// <exception cref="YDotNetException">Another exception is pending.</exception>
    public Transaction WriteTransaction()
    {
        var handle = BranchChannel.WriteTransaction(Handle);

        if (handle == nint.Zero)
        {
            ThrowHelper.PendingTransaction();
            return default!;
        }

        return new Transaction(handle, doc);
    }

    /// <summary>
    ///     Starts a new read-only <see cref="Transaction" /> on this <see cref="Branch" /> instance.
    /// </summary>
    /// <returns>The <see cref="Transaction" /> to perform operations in the branch.</returns>
    /// <exception cref="YDotNetException">Another exception is pending.</exception>
    public Transaction ReadTransaction()
    {
        var handle = BranchChannel.ReadTransaction(Handle);

        if (handle == nint.Zero)
        {
            ThrowHelper.PendingTransaction();
            return default!;
        }

        return new Transaction(handle, doc);
    }
}
