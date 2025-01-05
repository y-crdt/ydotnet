using Microsoft.Extensions.Logging;
using YDotNet.Document;
using YDotNet.Server.Storage;

namespace YDotNet.Server.Internal;

internal sealed class DocumentCache(
    IDocumentStorage documentStorage,
    IDocumentCallback documentCallback,
    IDocumentManager documentManager,
    DocumentManagerOptions options,
    ILogger logger) : IAsyncDisposable
{
    private readonly Dictionary<string, Item> documents = new(StringComparer.Ordinal);
    private readonly SemaphoreSlim slimLock = new(1);

    private sealed class Item
    {
        required public DocumentContainer Document { get; init; }

        public DateTime ValidUntil { get; set; }
    }

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

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
        // Keep the lock as short as possible.
        var toCleanup = GetItemsToRemove();

        foreach (var document in toCleanup)
        {
            // This is thread safe and will block all pending updates.
            await document.DisposeAsync().ConfigureAwait(false);

            await slimLock.WaitAsync().ConfigureAwait(false);
            try
            {
                // Remove the item after it has been removed.
                // If another request is making an update while the cleanup is running it will wait and then
                // abort with ObjectDisposedException.
                documents.Remove(document.Name);
            }
            finally
            {
                slimLock.Release();
            }
        }
    }

    private List<DocumentContainer> GetItemsToRemove()
    {
        var removed = new List<DocumentContainer>();

        slimLock.Wait();
        try
        {
            var now = Clock();

            foreach (var (_, item) in documents)
            {
                if (item.ValidUntil < now)
                {
                    removed.Add(item.Document);
                }
            }
        }
        finally
        {
            slimLock.Release();
        }

        return removed;
    }

    public async ValueTask<T> ApplyUpdateReturnAsync<T>(string name, Func<Doc, Task<T>> action)
    {
        try
        {
            var container = GetContainer(name);

            return await container.ApplyUpdateReturnAsync(action).ConfigureAwait(false);
        }
        catch (ObjectDisposedException)
        {
            // This happens when the document has been disposed while we are waiting to get the lock.
            // Just get a new document from the cache and try it again.
            var container = GetContainer(name);

            return await container.ApplyUpdateReturnAsync(action).ConfigureAwait(false);
        }
    }

    private DocumentContainer GetContainer(string name)
    {
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
