namespace YDotNet.Server;

public interface IDocumentCallback
{
    ValueTask OnDocumentChangingAsync(DocumentChangeEvent @event)
    {
        return default;
    }

    ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        return default;
    }

    ValueTask OnDocumentStoringAsync(DocumentStoreEvent @event)
    {
        return default;
    }

    ValueTask OnDocumentStoredAsync(DocumentStoreEvent @event)
    {
        return default;
    }

    ValueTask OnClientConnectedAsync(ClientConnectedEvent @event)
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
