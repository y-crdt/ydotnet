using Microsoft.Extensions.Logging;
using YDotNet.Document;

namespace YDotNet.Server.Internal;

internal sealed class DocumentContainer
{
    private readonly DocumentWriter documentWriter;
    private readonly DocumentManagerOptions options;
    private readonly string documentName;
    private readonly Task<Doc> loadingTask;
    private readonly SemaphoreSlim slimLock = new(1);

    public string Name => documentName;

    public DocumentContainer(
        string documentName,
        DocumentWriter documentWriter,
        IDocumentCallback documentCallback,
        IDocumentManager documentManager,
        DocumentManagerOptions options,
        ILogger logger)
    {
        this.documentName = documentName;
        this.documentWriter = documentWriter;
        this.options = options;

        loadingTask = LoadInternalAsync(documentCallback, documentManager, logger);
    }

    private async Task<Doc> LoadInternalAsync(IDocumentCallback documentCallback, IDocumentManager documentManager, ILogger logger)
    {
        var doc = await LoadCoreAsync().ConfigureAwait(false);

        await documentCallback.OnDocumentLoadedAsync(new DocumentLoadEvent
        {
            Document = doc,
            Context = new DocumentContext(documentName, 0),
            Source = documentManager,
        }).ConfigureAwait(false);

        doc.ObserveUpdatesV1(e =>
        {
            logger.LogDebug("Document {name} updated.", documentName);

            _ = documentWriter.WriteAsync(documentName, this);
        });

        return doc;
    }

    private async Task<Doc> LoadCoreAsync()
    {
        var documentData = await documentWriter.GetDocAsync(documentName).ConfigureAwait(false);

        if (documentData != null)
        {
            var document = new Doc(new()
            {
                SkipGarbageCollection = true,
            });

            using (var transaction = document.WriteTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be acquired.");
                }

                transaction.ApplyV1(documentData);
            }

            return document;
        }

        if (options.AutoCreateDocument)
        {
            return new(new()
            {
                SkipGarbageCollection = true,
            });
        }

        throw new InvalidOperationException("Document does not exist yet.");
    }

    public async Task DisposeAsync()
    {
        var document = await loadingTask.ConfigureAwait(false);

        await slimLock.WaitAsync().ConfigureAwait(false);
        try
        {
            try
            {
                using var transaction = document.WriteTransaction();

                await documentWriter.WriteAsync(documentName, transaction.StateDiffV1(stateVector: null)).ConfigureAwait(false);
            }
            finally
            {
                document?.Dispose();
            }
        }
        finally
        {
            slimLock.Release();
        }
    }

    public async Task<T> ApplyUpdateReturnAsync<T>(Func<Doc, Task<T>> action)
    {
        var document = await loadingTask.ConfigureAwait(false);

        await slimLock.WaitAsync().ConfigureAwait(false);
        try
        {
            return await action(document).ConfigureAwait(false);
        }
        finally
        {
            slimLock.Release();
        }
    }

    internal async Task<byte[]> GetStateAsync()
    {
        var document = await loadingTask.ConfigureAwait(false);

        await slimLock.WaitAsync().ConfigureAwait(false);
        try
        {
            using var transaction = document.ReadTransaction();

            var snapshot = transaction.Snapshot()!;

            return transaction.EncodeStateFromSnapshotV2(snapshot)!;
        }
        finally
        {
            slimLock.Release();
        }
    }
}
