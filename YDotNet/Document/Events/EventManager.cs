namespace YDotNet.Document.Events;

internal sealed class EventManager
{
    private readonly HashSet<IEventSubscriber> activeSubscribers = new();

    public void Register(IEventSubscriber eventSubscriber)
    {
        activeSubscribers.Add(eventSubscriber);
    }

    public void Unregister(IEventSubscriber eventSubscriber)
    {
        activeSubscribers.Remove(eventSubscriber);
    }

    public void Clear()
    {
        foreach (var subscriber in activeSubscribers)
        {
            subscriber.Clear();
        }
    }
}
