namespace YDotNet.Document.Events;

internal class EventSubscriber<TEvent>
{
    private readonly EventPublisher<TEvent> publisher = new EventPublisher<TEvent>();
    private readonly nint owner;
    private readonly Func<nint, Action<TEvent>, (uint Handle, object Callback)> subscribe;
    private readonly Action<nint, uint> unsubscribe;
    private (uint Handle, object Callback) nativeSubscription;

    public EventSubscriber(
        nint owner,
        Func<nint, Action<TEvent>, (uint Handle, object Callback)> subscribe,
        Action<nint, uint> unsubscribe)
    {
        this.owner = owner;
        this.subscribe = subscribe;
        this.unsubscribe = unsubscribe;
    }

    public void Clear()
    {
        publisher.Clear();
    }

    public IDisposable Subscribe(Action<TEvent> handler)
    {
        if (nativeSubscription.Handle == nint.Zero)
        {
            nativeSubscription = subscribe(owner, publisher.Publish);
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

        if (publisher.Count == 0 && nativeSubscription.Handle != nint.Zero)
        {
            unsubscribe(owner, nativeSubscription.Handle);

            nativeSubscription = default;
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
