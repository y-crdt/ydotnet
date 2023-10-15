namespace YDotNet.Document.Events;

internal class EventSubscriber<TEvent> : IEventSubscriber
{
    private readonly EventPublisher<TEvent> publisher = new();
    private readonly EventManager manager;
    private readonly nint owner;
    private readonly Func<nint, Action<TEvent>, (uint Handle, object Callback)> subscribe;
    private readonly Action<nint, uint> unsubscribe;
    private (uint Handle, object? Callback) nativeSubscription;

    public EventSubscriber(
        EventManager manager,
        nint owner,
        Func<nint, Action<TEvent>, (uint Handle, object Callback)> subscribe,
        Action<nint, uint> unsubscribe)
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
            nativeSubscription = subscribe(owner, publisher.Publish);

            // Register the subscriber as active in the manager.
            manager.Register(this);
        }

        publisher.Subscribe(handler);

        return new DelegateDisposable(() =>
        {
            Unsubscribe(handler);
        });
    }

    private void Unsubscribe(Action<TEvent> handler)
    {
        publisher.Unsubscribe(handler);

        UnsubscribeWhenSubscribed();
    }

    private void UnsubscribeWhenSubscribed()
    {
        // If this is the last subscription we can unubscribe from the native source again.
        if (publisher.Count == 0 && nativeSubscription.Callback != null)
        {
            unsubscribe(owner, nativeSubscription.Handle);

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
