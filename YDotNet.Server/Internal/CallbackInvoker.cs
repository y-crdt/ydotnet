using Microsoft.Extensions.Logging;

namespace YDotNet.Server.Internal;

public sealed class CallbackInvoker(IEnumerable<IDocumentCallback> callbacks, ILogger logger) : IDocumentCallback
{
    private readonly List<IDocumentCallback> callbacks = callbacks.ToList();

    public ValueTask OnInitializedAsync(IDocumentManager manager)
    {
        return InvokeCallbackAsync(manager, (c, m) => c.OnInitializedAsync(m));
    }

    public ValueTask OnDocumentLoadedAsync(DocumentLoadEvent @event)
    {
        return InvokeCallbackAsync(@event, (c, e) => c.OnDocumentLoadedAsync(e));
    }

    public ValueTask OnDocumentChangedAsync(DocumentChangedEvent @event)
    {
        return InvokeCallbackAsync(@event, (c, e) => c.OnDocumentChangedAsync(e));
    }

    public ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent @event)
    {
        return InvokeCallbackAsync(@event, (c, e) => c.OnClientDisconnectedAsync(e));
    }

    public ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        return InvokeCallbackAsync(@event, (c, e) => c.OnAwarenessUpdatedAsync(e));
    }

    private async ValueTask InvokeCallbackAsync<T>(T @event, Func<IDocumentCallback, T, ValueTask> action)
    {
        foreach (var callback in callbacks)
        {
            try
            {
                await action(callback, @event).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to invoke callback for {callback} and {event}.", callback, @event);
            }
        }
    }
}
