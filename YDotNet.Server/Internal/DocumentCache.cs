using Microsoft.Extensions.Logging;
using YDotNet.Document;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Internal;

internal sealed class DocumentCache : IAsyncDisposable
{
    private readonly ILogger logger;
    private readonly IDocumentCallback documentCallback;
    private readonly IDocumentManager documentManager;
    private readonly DocumentManagerOptions options;
    private readonly DocumentWriter documentWriter;
    private readonly Dictionary<string, Item> documents = new(StringComparer.Ordinal);
    private readonly SemaphoreSlim slimLock = new(1);

    private sealed class Item
    {
        required public DocumentContainer Document { get; init; }

        public DateTime ValidUntil { get; set; }
    }

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    public DocumentCache(
        IDocumentStorage documentStorage,
        IDocumentCallback documentCallback,
        IDocumentManager documentManager,
        DocumentManagerOptions options,
        ILogger logger)
    {
        this.documentCallback = documentCallback;
        this.documentManager = documentManager;
        this.documentWriter = new DocumentWriter(documentStorage, options.StoreDebounce, options.MaxWriteTimeInterval, logger);
        this.options = options;
        this.logger = logger;
    }

    public async ValueTask DisposeAsync()
    {
        await slimLock.WaitAsync().ConfigureAwait(false);
        try
        {
            foreach (var (_, item) in documents)
            {
                await item.Document.DisposeAsync().ConfigureAwait(false);
            }

            documents.Clear();
        }
        finally
        {
            slimLock.Release();
        }
    }

    public async Task RemoveEvictedItemsAsync()
    {
        foreach (var document in RemoveItems())
        {
            await document.DisposeAsync().ConfigureAwait(false);

            await slimLock.WaitAsync().ConfigureAwait(false);
            try
            {
                documents.Remove(document.Name);
            }
            finally
            {
                slimLock.Release();
            }
        }
    }

    private IEnumerable<DocumentContainer> RemoveItems()
    {
        List<DocumentContainer>? removed = null;

        slimLock.Wait();
        try
        {
            if (documents.Count == 0)
            {
                return Enumerable.Empty<DocumentContainer>();
            }

            var now = Clock();

            foreach (var (key, item) in documents)
            {
                if (item.ValidUntil < now)
                {
                    removed ??= new();
                    removed.Add(item.Document);
                }
            }
        }
        finally
        {
            slimLock.Release();
        }

        return removed ?? Enumerable.Empty<DocumentContainer>();
    }

    public async ValueTask<T> ApplyUpdateReturnAsync<T>(string name, Func<Doc, Task<T>> action)
    {
        try
        {
            var container = GetContext(name);

            return await container.ApplyUpdateReturnAsync(action).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            var container = GetContext(name);

            return await container.ApplyUpdateReturnAsync(action).ConfigureAwait(false);
        }
    }

    private DocumentContainer GetContext(string name)
    {
        // The memory cache does not guarantees that the callback is called in parallel. Therefore we need the lock here.
        slimLock.Wait();
        try
        {
            if (!documents.TryGetValue(name, out var item))
            {
                var document = new DocumentContainer(
                    name,
                    documentWriter,
                    documentCallback,
                    documentManager,
                    options,
                    logger);

                item = new Item
                {
                    Document = document,
                };

                documents[name] = item;
            }

            item.ValidUntil = Clock() + options.CacheDuration;
            return item.Document;
        }
        finally
        {
            slimLock.Release();
        }
    }
}
