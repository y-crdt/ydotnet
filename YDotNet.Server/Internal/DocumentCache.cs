using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Internal;

internal sealed class DocumentCache : IAsyncDisposable
{
    private readonly ILogger logger;
    private readonly IDocumentStorage documentStorage;
    private readonly IDocumentCallback documentCallback;
    private readonly IDocumentManager documentManager;
    private readonly DocumentManagerOptions options;
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
        this.documentStorage = documentStorage;
        this.documentCallback = documentCallback;
        this.documentManager = documentManager;
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
                await item.Document.FlushAsync().ConfigureAwait(false);
            }

            documents.Clear();
        }
        finally
        {
            slimLock.Release();
        }
    }

    public async Task RemoveEvictedItems()
    {
        foreach (var document in RemoveItems())
        {
            await document.FlushAsync().ConfigureAwait(false);
        }
    }

    private IEnumerable<DocumentContainer> RemoveItems()
    {
        List<(string, DocumentContainer)>? removed = null;

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
                    removed.Add((key, item.Document));
                }
            }

            if (removed != null)
            {
                foreach (var (key, _) in removed)
                {
                    documents.Remove(key);
                }
            }
        }
        finally
        {
            slimLock.Release();
        }

        return removed?.Select(x => x.Item2) ?? Enumerable.Empty<DocumentContainer>();
    }

    public DocumentContainer GetContext(string name)
    {
        // The memory cache does not guarantees that the callback is called in parallel. Therefore we need the lock here.
        slimLock.Wait();
        try
        {
            if (!documents.TryGetValue(name, out var item))
            {
                var document = new DocumentContainer(
                    name,
                    documentStorage,
                    documentCallback,
                    documentManager,
                    options,
                    logger);

                item = new Item
                {
                    Document = document,
                };
            }

            return memoryCache.GetOrCreate(name, entry =>
            {
                // Check if there are any pending flushes. If the flush is still running we wait for the flush.
                if (documents.TryGetValue(name, out var container))
                {
                    documents.Remove(name);
                }
                else
                {
                    
                }

                // For each access we extend the lifetime of the cache entry.
                entry.SlidingExpiration = options.CacheDuration;
                entry.RegisterPostEvictionCallback((_, _, _, _) =>
                {
                    // There is no background thread for eviction. It is just done from sync code.
                    _ = CleanupAsync(name, container);
                });

                documents.Add(name, container);
                return container;
            })!;
        }
        finally
        {
            slimLock.Release();
        }
    }

    private async Task CleanupAsync(string name, DocumentContainer context)
    {
        logger.LogDebug("Cleanup document {document}", name);

        // Flush all pending changes to the storage and then remove the context from the list of living entries.
        await context.FlushAsync().ConfigureAwait(false);

        await slimLock.WaitAsync().ConfigureAwait(false);
        try
        {
            documents.Remove(name);
        }
        finally
        {
            slimLock.Release();
        }
    }
}
