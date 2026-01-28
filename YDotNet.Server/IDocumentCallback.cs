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

    ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        return default;
    }

    ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent @event)
    {
        return default;
    }

    ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        return default;
    }

    ValueTask OnClientConnectedAsync(ClientConnectedEvent @event)
    {
        return default;
    }

    ValueTask OnDocumentOffloadedAsync(DocumentOffloadedEvent @event)
    {
        return default;
    }
}
