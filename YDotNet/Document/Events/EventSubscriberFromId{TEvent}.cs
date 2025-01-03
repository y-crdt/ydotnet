﻿using YDotNet.Document.Types.Branches;

namespace YDotNet.Document.Events;

internal class EventSubscriberFromId<TEvent>(
    EventManager manager,
    Branch owner,
    Func<nint, Action<TEvent>, (nint Handle, object Callback)> subscribe,
    Action<nint> unsubscribe) : IEventSubscriber
{
    private readonly EventPublisher<TEvent> publisher = new();
    private (nint Handle, object? Callback) nativeSubscription;

    public void Clear()
    {
        this.publisher.Clear();

        this.UnsubscribeWhenSubscribed();
    }

    public IDisposable Subscribe(Action<TEvent> handler)
    {
        // If this is the first native subscription, subscribe to the actual source by invoking the action.
        if (this.nativeSubscription.Callback == null)
        {
            using (var transaction = owner.ReadTransaction())
            {
                var handle = owner.GetHandle(transaction);

                this.nativeSubscription = subscribe(handle, this.publisher.Publish);
            }

            // Register the subscriber as active in the manager.
            manager.Register(this);
        }

        this.publisher.Subscribe(handler);

        return new DelegateDisposable(() => this.Unsubscribe(handler));
    }

    private void Unsubscribe(Action<TEvent> handler)
    {
        this.publisher.Unsubscribe(handler);

        this.UnsubscribeWhenSubscribed();
    }

    private void UnsubscribeWhenSubscribed()
    {
        // If this is the last subscription, we can unsubscribe from the native source again.
        if (this.publisher.Count == 0 && this.nativeSubscription.Callback != null)
        {
            unsubscribe(this.nativeSubscription.Handle);

            this.nativeSubscription = default;

            // The manager will clear all active subscriptions when the document where the manager belongs to is disposed.
            manager.Unregister(this);
        }
    }

    private sealed record DelegateDisposable(Action Delegate) : IDisposable
    {
        public void Dispose()
        {
            this.Delegate();
        }
    }
}
