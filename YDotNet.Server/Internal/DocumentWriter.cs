using System.Reactive.Concurrency;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Internal;

internal class DocumentWriter : IAsyncDisposable
{
    private readonly ActionBlock<(WriteMessage, bool)> delayBlock;
    private readonly ActionBlock<WriteMessage> writeBlock;
    private readonly IDocumentStorage storage;

    private abstract record WriteMessage(string Name);

    private sealed record WriteDataMessage(string Name, byte[] State) : WriteMessage(Name);

    private sealed record WriteDocumentMessage(string Name, DocumentContainer Container) : WriteMessage(Name);

    private record LastMessage
    {
        public IDisposable? Timer { get; set; }

        public WriteMessage? Message { get; set; }

        required public DateTimeOffset Created { get; init; }
    }

    public DocumentWriter(
        IDocumentStorage storage,
        TimeSpan delay,
        TimeSpan delayMax,
        ILogger logger)
    {
        this.storage = storage;

        writeBlock = new ActionBlock<WriteMessage>(
            async message =>
            {
                logger.LogDebug("Document {documentName} will be written to storage {storage}", message.Name, storage);

                try
                {
                    byte[] state = Array.Empty<byte>();
                    if (message is WriteDataMessage dataMessage)
                    {
                        state = dataMessage.State;
                    }
                    else if (message is WriteDocumentMessage documentMessage)
                    {
                        state = await documentMessage.Container.GetStateAsync().ConfigureAwait(false);
                    }

                    await storage.StoreDocAsync(message.Name, state).ConfigureAwait(false);

                    logger.LogDebug("Document {documentName} has been written to storage {storage}", message.Name, storage);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Document {documentName} failed to write to storage {storage}", message.Name, storage);
                }
            },
            dataflowBlockOptions: new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 16,
                MaxMessagesPerTask = 1,
            })
        {
        };
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

    public Task WriteAsync(string name, DocumentContainer document)
    {
        return delayBlock.SendAsync((new WriteDocumentMessage(name, document), false));
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

    public ValueTask<byte[]?> GetDocAsync(string documentName)
    {
        return storage.GetDocAsync(documentName);
    }
}
