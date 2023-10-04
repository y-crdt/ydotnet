using YDotNet.Document;

namespace YDotNet.Server.Internal;

internal sealed class SubscribeToUpdatesV1Once : IDisposable
{
    private readonly Action unsubscribe;

    public byte[]? Update { get; private set; }

    public SubscribeToUpdatesV1Once(Doc doc)
    {
        var subscription = doc.ObserveUpdatesV1(@event =>
        {
            Update = @event.Update;
        });

        unsubscribe = () =>
        {
            doc.UnobserveUpdatesV1(subscription);
        };
    }

    public void Dispose()
    {
        unsubscribe();
    }
}
