using YDotNet.Document;
using YDotNet.Server.Storage;
using YDotNet.Server.Utils;

namespace YDotNet.Server;

internal sealed class DocumentContext
{
    private readonly IDocumentStorage documentStorage;
    private readonly DocumentManagerOptions options;
    private readonly string documentName;
    private readonly Task<Doc> loadingTask;
    private readonly SemaphoreSlim slimLock = new SemaphoreSlim(1);
    private readonly DelayedWriter writer;
    private Doc? doc;

    public DocumentContext(string documentName, IDocumentStorage documentStorage, DocumentManagerOptions options)
    {
        this.documentName = documentName;
        this.documentStorage = documentStorage;
        this.options = options;

        writer = new DelayedWriter(options.DelayWriting, options.MaxWriteTimeInterval, WriteAsync);

        loadingTask = LoadInternalAsync();
    }

    public Task FlushAsync()
    {
        return writer.FlushAsync();
    }

    private async Task<Doc> LoadInternalAsync()
    {
        doc = await LoadCoreAsync();

        doc.ObserveUpdatesV2(e =>
        {
            writer.Ping();
        });

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

    private async Task WriteAsync()
    {
        var doc = this.doc;

        if (doc == null)
        {
            return;
        }

        byte[] snapshot;

        slimLock.Wait();
        try
        {
            using var transaction = doc.ReadTransaction();

            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction cannot be acquired.");
            }

            snapshot = transaction!.Snapshot();
        }
        finally
        {
            slimLock.Release();
        }

        await documentStorage.StoreDocAsync(documentName, snapshot);
    }
}
