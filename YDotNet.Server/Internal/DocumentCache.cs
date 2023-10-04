using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Internal;

internal sealed class DocumentContainerCache : IAsyncDisposable
{
    private readonly IDocumentStorage documentStorage;
    private readonly IDocumentCallback documentCallback;
    private readonly IDocumentManager documentManager;
    private readonly DocumentManagerOptions options;
    private readonly MemoryCache memoryCache = new(Options.Create(new MemoryCacheOptions()));
    private readonly Dictionary<string, DocumentContainer> livingContainers = new();
    private readonly SemaphoreSlim slimLock = new(1);

    public DocumentContainerCache(
        IDocumentStorage documentStorage,
        IDocumentCallback documentCallback,
        IDocumentManager documentManager,
        DocumentManagerOptions options)
    {
        this.documentStorage = documentStorage;
        this.documentCallback = documentCallback;
        this.documentManager = documentManager;
        this.options = options;
    }

    public async ValueTask DisposeAsync()
    {
        await slimLock.WaitAsync();
        try
        {
            foreach (var (_, container) in livingContainers)
            {
                await container.FlushAsync();
            }
        }
        finally
        {
            slimLock.Release();
        }
    }

    public void RemoveEvictedItems()
    {
        memoryCache.Remove(true);
    }

    public DocumentContainer GetContext(string name)
    {
        // The memory cache does nto guarantees that the callback is called in parallel. Therefore we need the lock here.
        slimLock.Wait();
        try
        {
            return memoryCache.GetOrCreate(name, entry =>
            {
                // Check if there are any pending flushes. If the flush is still running we reuse the context.
                if (livingContainers.TryGetValue(name, out var container))
                {
                    livingContainers.Remove(name);
                }
                else
                {
                    container = new DocumentContainer(
                        name, 
                        documentStorage, 
                        documentCallback,
                        documentManager,
                        options);
                }

                // For each access we extend the lifetime of the cache entry.
                entry.SlidingExpiration = options.CacheDuration;
                entry.RegisterPostEvictionCallback((_, _, _, _) =>
                {
                    // There is no background thread for eviction. It is just done from 
                    _ = CleanupAsync(name, container);
                });

                livingContainers.Add(name, container);
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
        // Flush all pending changes to the storage and then remove the context from the list of living entries.
        await context.FlushAsync();

        slimLock.Wait();
        try
        {
            livingContainers.Remove(name);
        }
        finally
        {
            slimLock.Release();
        }
    }
}
