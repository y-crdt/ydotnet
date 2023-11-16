using YDotNet.Document;

namespace YDotNet.Server.Internal;

internal sealed class SubscribeToUpdatesV1Once : IDisposable
{
    private readonly IDisposable unsubscribe;

    public SubscribeToUpdatesV1Once(Doc doc)
    {
        unsubscribe = doc.ObserveUpdatesV1(@event =>
        {
            Update = @event.Update;
        });
    }

    public byte[]? Update { get; private set; }

    public void Dispose()
    {
        unsubscribe.Dispose();
    }
}
