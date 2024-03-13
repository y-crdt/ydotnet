using System.Reactive.Concurrency;
using System.Threading.Tasks.Dataflow;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Internal;

internal class BatchWriter : IAsyncDisposable
{
    private readonly ActionBlock<(WriteMessage, bool)> delayBlock;
    private readonly ActionBlock<WriteMessage> writeBlock;

    private record WriteMessage(string Name);

    private record WriteDataMessage(string Name, byte[] State) : WriteMessage(Name);

    private record WriteDocumentMessage(string Name, DocumentContainer Container);

    private record LastMessage
    {
        public IDisposable? Timer { get; set; }

        public WriteMessage? Message { get; set; }

        required public DateTimeOffset Created { get; init; }
    }

    public BatchWriter(
        IDocumentStorage storage,
        TimeSpan delay,
        TimeSpan delayMax)
    {
        writeBlock = new ActionBlock<WriteMessage>(
            async message =>
            {
                if (message is WriteDataMessage dataMessage)
                {
                    await storage.StoreDocAsync(dataMessage.Name, dataMessage.State).ConfigureAwait(false);
                }
            },
            dataflowBlockOptions: new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 16,
                MaxMessagesPerTask = 1,
            });

        delayBlock = CreateDelayBlock(writeBlock, delay, delayMax);
    }

    public async ValueTask DisposeAsync()
    {
        delayBlock.Complete();
        await delayBlock.Completion.ConfigureAwait(false);

        writeBlock.Complete();
        await writeBlock.Completion.ConfigureAwait(false);
    }

    public Task WriteAsync(string name, byte[] doc)
    {
        return delayBlock.SendAsync((new WriteDataMessage(name, doc), false));
    }

    private ActionBlock<(WriteMessage Message, bool Force)> CreateDelayBlock(ActionBlock<WriteMessage> target, TimeSpan delay, TimeSpan maxDelay)
    {
        var byName = new Dictionary<string, LastMessage>(StringComparer.Ordinal);

        var scheduler = Scheduler.Default;

        return new ActionBlock<(WriteMessage, bool)>(
            async item =>
            {
                var (message, force) = item;

                if (force)
                {
                    byName.Remove(message.Name);
                    await target.SendAsync(message).ConfigureAwait(false);
                    return;
                }

                if (byName.TryGetValue(message.Name, out var existing))
                {
                    if ((scheduler.Now - existing.Created > maxDelay) || (force && existing.Message == message))
                    {
                        byName.Remove(message.Name);
                        await target.SendAsync(message).ConfigureAwait(false);
                        return;
                    }
                }
                else
                {
                    existing = new LastMessage
                    {
                        Created = scheduler.Now,
                    };

                    byName[message.Name] = existing;
                }

                existing.Message = message;
                existing.Timer?.Dispose();
                existing.Timer = scheduler.Schedule(delay, () =>
                {
                    _ = delayBlock.SendAsync((message, true));
                });
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1,
            });
    }
}
