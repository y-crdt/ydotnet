using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using YDotNet.Server.Storage;

namespace YDotNet.Server;

internal sealed class DocumentContextCache : IAsyncDisposable
{
    private readonly IDocumentStorage documentStorage;
    private readonly DocumentManagerOptions options;
    private readonly MemoryCache memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
    private readonly List<DocumentContext> livingContexts = new List<DocumentContext>();
    private readonly SemaphoreSlim slimLock = new SemaphoreSlim(1);

    public DocumentContextCache(IDocumentStorage documentStorage, DocumentManagerOptions options)
    {
        this.documentStorage = documentStorage;
        this.options = options;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var documentContext in livingContexts)
        {
            await documentContext.DisposeAsync();
        }
    }

    public DocumentContext GetContext(string documentName)
    {
        slimLock.Wait();
        try
        {
            return memoryCache.GetOrCreate(documentName, entry =>
            {
                var context = new DocumentContext(documentName, documentStorage, options);

                entry.SlidingExpiration = options.CacheDuration;
                entry.RegisterPostEvictionCallback((_, _, _, _) =>
                {
                    livingContexts.Remove(context);
                });

                livingContexts.Add(context);
                return context;
            })!;
        }
        finally
        {
            slimLock.Release();
        }
    }
}
