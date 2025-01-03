namespace YDotNet.Document.Events;

internal class EventSubscriber<TEvent>(
    EventManager manager,
    nint owner,
    Func<nint, Action<TEvent>, (nint Handle, object Callback)> subscribe,
    Action<nint> unsubscribe) : IEventSubscriber
{
    private readonly EventPublisher<TEvent> publisher = new();
    private (nint Handle, object? Callback) nativeSubscription;

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
            nativeSubscription = subscribe(owner, publisher.Publish);

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
