using YDotNet.Document.Types.Events;
using YDotNet.Server;

namespace Demo;

public sealed class Listener : IDocumentCallback
{
    private readonly ILogger<Listener> log;

    public Listener(ILogger<Listener> log)
    {
        this.log = log;
    }

    public ValueTask OnDocumentLoadedAsync(DocumentLoadEvent @event)
    {
        var map = @event.Document.Map("increment");

        map?.ObserveDeep(changes =>
        {
            foreach (var change in changes)
            {
                var key = change.MapEvent?.Keys.FirstOrDefault(x => x.Key == "value" && x.Tag != EventKeyChangeTag.Remove);

                if (key != null)
                {
                    var valueOld = key.OldValue?.Double;
                    var valueNew = key.NewValue?.Double;

                    if (valueOld == valueNew)
                    {
                        continue;
                    }

                    log.LogInformation("Counter updated from {oldValue} to {newValue}.", valueOld, valueNew);
                }
            }
        });
       

        return default;
    }
}
