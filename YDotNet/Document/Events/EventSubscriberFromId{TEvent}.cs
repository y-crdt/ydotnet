using System.Collections.Concurrent;
using YDotNet.Document.Types.Branches;

namespace YDotNet.Document.Events;

internal class EventSubscriberFromId<TEvent> : IEventSubscriber
{
    private readonly EventManager manager;
    private readonly Branch owner;
    private readonly EventPublisher<TEvent> publisher = new();
    private readonly Func<nint, Action<TEvent>, (nint Handle, object Callback)> subscribe;
    private readonly Action<nint> unsubscribe;
    private (nint Handle, object? Callback) nativeSubscription;

    // The native callback, returned by `subscribe`, must be stored so it doesn't
    // throw exceptions later if the Rust side invokes it after it's been collected.
    public EventSubscriberFromId(
        EventManager manager,
        Branch owner,
        Func<nint, Action<TEvent>, (nint Handle, object Callback)> subscribe,
        Action<nint> unsubscribe)
    {
        this.manager = manager;
        this.owner = owner;
        this.subscribe = subscribe;
        this.unsubscribe = unsubscribe;
    }

    public void Clear()
    {
        publisher.Clear();

        UnsubscribeWhenSubscribed();
    }

    public IDisposable Subscribe(Action<TEvent> handler)
    {
        // If this is the first native subscription, subscribe to the actual source by invoking the action.
        if (nativeSubscription.Callback == null)
        {
            using (var transaction = owner.ReadTransaction())
            {
                var handle = owner.BranchId.GetHandle(transaction);

                nativeSubscription = subscribe(handle, publisher.Publish);
            }

            // Register the subscriber as active in the manager.
            manager.Register(this);
        }

        publisher.Subscribe(handler);

        return new DelegateDisposable(() => Unsubscribe(handler));
    }

    private void Unsubscribe(Action<TEvent> handler)
    {
        publisher.Unsubscribe(handler);

        UnsubscribeWhenSubscribed();
    }

    private void UnsubscribeWhenSubscribed()
    {
        // If this is the last subscription, we can unsubscribe from the native source again.
        if (publisher.Count == 0 && nativeSubscription.Callback != null)
        {
            unsubscribe(nativeSubscription.Handle);

            nativeSubscription = default;

            // The manager will clear all active subscriptions when the document where the manager belongs to is disposed.
            manager.Unregister(this);
        }
    }

    private sealed record DelegateDisposable(Action Delegate) : IDisposable
    {
        public void Dispose()
        {
            Delegate();
        }
    }
}
