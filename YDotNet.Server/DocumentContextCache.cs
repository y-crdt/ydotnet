using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using YDotNet.Server.Storage;

namespace YDotNet.Server;

internal sealed class DocumentContextCache : IAsyncDisposable
{
    private readonly IDocumentStorage documentStorage;
    private readonly DocumentManagerOptions options;
    private readonly MemoryCache memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
    private readonly Dictionary<string, DocumentContext> livingContexts = new Dictionary<string, DocumentContext>();
    private readonly SemaphoreSlim slimLock = new SemaphoreSlim(1);

    public DocumentContextCache(IDocumentStorage documentStorage, DocumentManagerOptions options)
    {
        this.documentStorage = documentStorage;
        this.options = options;
    }

    public async ValueTask DisposeAsync()
    {
        await slimLock.WaitAsync();
        try
        {
            foreach (var (_, context) in livingContexts)
            {
                await context.FlushAsync();
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

    public DocumentContext GetContext(string name)
    {
        // The memory cache does nto guarantees that the callback is called in parallel. Therefore we need the lock here.
        slimLock.Wait();
        try
        {
            return memoryCache.GetOrCreate(name, entry =>
            {
                // Check if there are any pending flushes. If the flush is still running we reuse the context.
                if (livingContexts.TryGetValue(name, out var context))
                {
                    livingContexts.Remove(name);
                }
                else
                {
                    context = new DocumentContext(name, documentStorage, options);
                }

                // For each access we extend the lifetime of the cache entry.
                entry.SlidingExpiration = options.CacheDuration;
                entry.RegisterPostEvictionCallback((_, _, _, _) =>
                {
                    // There is no background thread for eviction. It is just done from 
                    _ = CleanupAsync(name, context);
                });

                livingContexts.Add(name, context);
                return context;
            })!;
        }
        finally
        {
            slimLock.Release();
        }
    }

    private async Task CleanupAsync(string name, DocumentContext context)
    {
        // Flush all pending changes to the storage and then remove the context from the list of living entries.
        await context.FlushAsync();

        slimLock.Wait();
        try
        {
            livingContexts.Remove(name);
        }
        finally
        {
            slimLock.Release();
        }
    }
}
