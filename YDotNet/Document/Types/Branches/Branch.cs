using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     The generic type that can be used to refer to all shared data type instances.
/// </summary>
// TODO: Implement `ytype_kind`.
public abstract class Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Branch" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    protected Branch(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
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
    public EventSubscription ObserveDeep(Action<IEnumerable<EventBranch>> action)
    {
        var subscriptionId = BranchChannel.ObserveDeep(
            Handle,
            nint.Zero,
            (state, length, eventsHandle) =>
            {
                var events = MemoryReader.TryReadIntPtrArray(eventsHandle, length, size: 24)!
                    .Select(x => new EventBranch(x))
                    .ToArray();

                action(events);
            });

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed within <see cref="Branch" /> scope.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveDeep(EventSubscription subscription)
    {
        BranchChannel.UnobserveDeep(Handle, subscription.Id);
    }
}
