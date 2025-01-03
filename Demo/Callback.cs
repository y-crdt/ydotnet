using System.Text.Json.Serialization;
using YDotNet.Document.Types.Events;
using YDotNet.Extensions;
using YDotNet.Server;

namespace Demo;

public sealed class Callback(ILogger<Callback> log) : IDocumentCallback
{
    public ValueTask OnAwarenessUpdatedAsync(ClientAwarenessEvent @event)
    {
        log.LogInformation("Client {clientId} awareness changed.", @event.Context.ClientId);
        return default;
    }

    public ValueTask OnClientDisconnectedAsync(ClientDisconnectedEvent @event)
    {
        log.LogInformation("Client {clientId} disconnected.", @event.Context.ClientId);
        return default;
    }

    public ValueTask OnDocumentLoadedAsync(DocumentLoadEvent @event)
    {
        if (@event.Context.DocumentName == "notifications")
        {
            return default;
        }

        var map = @event.Document.Map("increment");

        map?.ObserveDeep(
            changes =>
            {
                foreach (var change in changes)
                {
                    var key = change.MapEvent?.Keys.FirstOrDefault(
                        x => x.Key == "value" && x.Tag != EventKeyChangeTag.Remove);

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

        var chat = @event.Document.Array("stream");

        chat?.ObserveDeep(
            async changes =>
            {
                var newNotificationsRaw =
                    changes
                        .Where(x => x.Tag == EventBranchTag.Array)
                        .Select(x => x.ArrayEvent)
                        .SelectMany(x => x.Delta.Where(x => x.Tag == EventChangeTag.Add))
                        .SelectMany(x => x.Values)
                        .ToArray();

                if (newNotificationsRaw.Length == 0)
                {
                    return;
                }

                await Task.Delay(millisecondsDelay: 100);

                var notificationCtx = new DocumentContext("notifications", ClientId: 0);

                await @event.Source.UpdateDocAsync(
                    notificationCtx, doc =>
                    {
                        List<Notification> notifications;

                        // Keep the transaction open as short as possible.
                        using (var transaction = @event.Document.ReadTransaction())
                        {
                            notifications = newNotificationsRaw.Select(x => x.To<Notification>(transaction)).ToList();
                        }

                        var array = doc.Array("stream");

                        notifications = notifications.Select(
                                x => new Notification { Text = $"You got the follow message: {x.Text}" })
                            .ToList();

                        // Keep the transaction open as short as possible.
                        using (var transaction = doc.WriteTransaction())
                        {
                            array.InsertRange(
                                transaction, array.Length(transaction),
                                notifications.Select(x => x.ToInput()).ToArray());
                        }
                    });
            });


        return default;
    }

    public sealed class Notification
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }
}
