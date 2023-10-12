namespace YDotNet.Document.Events;

internal sealed class EventPublisher<TEvent>
{
    private readonly HashSet<Action<TEvent>> subscriptions = new();

    public int Count => subscriptions.Count;

    public void Clear()
    {
        subscriptions.Clear();
    }

    public void Subscribe(Action<TEvent> handler)
    {
        subscriptions.Add(handler);
    }

    public void Unsubscribe(Action<TEvent> handler)
    {
        if (!subscriptions.Remove(handler))
        {
            return;
        }
    }

    public void Publish(TEvent @event)
    {
        foreach (var subscription in subscriptions)
        {
            try
            {
                subscription(@event);
            }
            catch
            {
                continue;
            }
        }
    }
}
