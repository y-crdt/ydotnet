using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Document.Events;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     The generic type that can be used to refer to all shared data type instances.
/// </summary>
public abstract class Branch : UnmanagedResource
{
    private readonly EventSubscriber<EventBranch[]> onDeep;

    internal protected Branch(nint handle, Doc doc, bool isDeleted)
        : base(handle, isDeleted)
    {
        Doc = doc;

#pragma warning disable CA1806 // Do not ignore method results
        onDeep = new EventSubscriber<EventBranch[]>(
            doc.EventManager,
            handle,
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
            (branch, s) => BranchChannel.UnobserveDeep(branch, s));
#pragma warning restore CA1806 // Do not ignore method results
    }

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

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        // Nothing should be done to dispose `Branch` instances (shared types).
        // They're disposed automatically when their parent `Doc` is disposed.
    }
}
