namespace YDotNet.Server;

public interface IDocumentCallback
{
    ValueTask OnInitializedAsync(IDocumentManager manager)
    {
        return default;
    }

    ValueTask OnDocumentLoadedAsync(DocumentLoadEvent @event)
    {
        return default;
    }

    ValueTask OnDocumentChangingAsync(DocumentChangeEvent @event)
    {
        return default;
    }

    ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        return default;
    }

    ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent[] events)
    {
        return default;
    }

    ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        return default;
    }
}
