using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Document.Events;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     The generic type that can be used to refer to all shared data type instances.
/// </summary>
public abstract class Branch
{
    private readonly EventSubscriberFromId<EventBranch[]> onDeep;

    protected internal Branch(nint handle, Doc doc, bool isDeleted)
    {
        Doc = doc;

        BranchId = BranchId.FromHandle(handle, isDeleted);

        onDeep = new EventSubscriberFromId<EventBranch[]>(
            doc.EventManager,
            this,
            (branch, action) =>
            {
                BranchChannel.ObserveCallback callback = (_, length, ev) =>
                {
                    var events = MemoryReader.ReadStructsWithHandles<EventBranchNative>(ev, length)
                        .Select(x => new EventBranch(x, doc))
                        .ToArray();

                    action(events);
                };

                return (BranchChannel.ObserveDeep(branch, nint.Zero, callback), callback);
            },
            SubscriptionChannel.Unobserve);
    }

    internal BranchId BranchId { get; }

    internal Doc Doc { get; }

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
        return onDeep.Subscribe(action);
    }

    /// <summary>
    ///     Starts a new read-write <see cref="Transaction" /> on this <see cref="Branch" /> instance.
    /// </summary>
    /// <returns>The <see cref="Transaction" /> to perform operations in the document.</returns>
    /// <exception cref="YDotNetException">Another write transaction has been created and not committed yet.</exception>
    public Transaction WriteTransaction()
    {
        return Doc.WriteTransaction();
    }

    /// <summary>
    ///     Starts a new read-only <see cref="Transaction" /> on this <see cref="Branch" /> instance.
    /// </summary>
    /// <returns>The <see cref="Transaction" /> to perform operations in the branch.</returns>
    /// <exception cref="YDotNetException">Another write transaction has been created and not committed yet.</exception>
    public Transaction ReadTransaction()
    {
        return Doc.ReadTransaction();
    }
}
