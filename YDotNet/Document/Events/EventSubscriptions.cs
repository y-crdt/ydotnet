using YDotNet.Infrastructure;

namespace YDotNet.Document.Events;

internal sealed class EventSubscriptions
{
    private readonly HashSet<EventSubscription> subscriptions = new HashSet<EventSubscription>();

    public IDisposable Add(object callback, Action unsubscribe)
    {
        var subscription = new EventSubscription(callback, s =>
        {
            subscriptions.Remove(s);
            unsubscribe();
        });

        subscriptions.Add(subscription);
        return subscription;
    }

    public void Clear()
    {
        subscriptions.Clear();
    }

    private sealed class EventSubscription : Resource
    {
        private readonly Action<EventSubscription> unsubscribe;

        internal EventSubscription(object callback, Action<EventSubscription> unsubscribe)
        {
            this.unsubscribe = unsubscribe;

            // Just holds a reference to the callback to prevent garbage collection.
            Callback = callback;
        }

        internal object? Callback { get; set; }

        protected override void DisposeCore(bool disposing)
        {
            unsubscribe(this);
        }
    }
}
