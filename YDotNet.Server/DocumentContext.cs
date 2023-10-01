using YDotNet.Document;
using YDotNet.Server.Storage;

namespace YDotNet.Server;

internal sealed class DocumentContext : IAsyncDisposable
{
    private readonly IDocumentStorage documentStorage;
    private readonly DocumentManagerOptions options;
    private readonly string documentName;
    private readonly Task<Doc> loadingTask;
    private readonly SemaphoreSlim slimLock = new SemaphoreSlim(1);
    private DateTime lastWrite;
    private Timer? writeTimer;
    private Task? writeTask;
    private Doc? doc;

    public DocumentContext(string documentName, IDocumentStorage documentStorage, DocumentManagerOptions options)
    {
        this.documentName = documentName;
        this.documentStorage = documentStorage;
        this.options = options;

        loadingTask = LoadInternalAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (writeTask != null)
        {
            await writeTask;
            return;
        }

        if (writeTimer != null && doc != null)
        {
            await WriteAsync(doc);
        }
    }

    private async Task<Doc> LoadInternalAsync()
    {
        var doc = await LoadCoreAsync();

        doc.ObserveUpdatesV2(e =>
        {
            var now = DateTime.UtcNow;

            if (lastWrite == default)
            {
                lastWrite = now;
            }

            if (writeTask != null)
            {
                var timeSinceLastWrite = now - lastWrite;
                if (timeSinceLastWrite > options.MaxWriteTimeInterval)
                {
                    Write(doc);
                }
                else
                {
                    writeTimer?.Dispose();
                    writeTimer = new Timer(_ => Write(doc), null, (int)options.DelayWriting.TotalMilliseconds, 0);
                }
            }
        });

        this.doc = doc;

        return doc;
    }

    private async Task<Doc> LoadCoreAsync()
    {
        var documentData = await documentStorage.GetDocAsync(documentName);

        if (documentData != null)
        {
            var doc = new Doc();

            using (var transaction = doc.WriteTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be acquired.");
                }

                transaction.ApplyV2(documentData);
            }

            return doc;
        }

        if (options.AutoCreateDocument)
        {
            return new Doc();
        }
        else
        {
            throw new InvalidOperationException("Document does not exist yet.");
        }
    }

    private void Write(Doc doc)
    {
        writeTask = WriteAsync(doc).ContinueWith(_ => writeTask = null);
    }

    private async Task WriteAsync(Doc doc)
    {
        using (var transaction = doc.ReadTransaction())
        {
            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction cannot be acquired.");
            }

            var state = transaction!.Snapshot();

            await documentStorage.StoreDocAsync(documentName, state);
        }

        lastWrite = DateTime.Now;
    }

    public async Task<T> ApplyUpdateReturnAsync<T>(Func<Doc, T> action)
    {
        var document = await loadingTask;

        slimLock.Wait();
        try
        {
            return action(document);
        }
        finally
        {
            slimLock.Release();
        }
    }
}
