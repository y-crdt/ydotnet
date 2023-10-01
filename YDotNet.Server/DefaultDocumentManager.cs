using Microsoft.Extensions.Options;
using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Server.Storage;

namespace YDotNet.Server;

public sealed class DefaultDocumentManager : IDocumentManager
{
    private DocumentContextCache contexts;

    public DefaultDocumentManager(IDocumentStorage documentStorage,
        IOptions<DocumentManagerOptions> options)
    {
        contexts = new DocumentContextCache(documentStorage, options.Value);
    }

    public async ValueTask<byte[]> GetMissingChanges(string name, byte[] stateVector)
    {
        var context = contexts.GetContext(name);

        return await context.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.ReadTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be created.");
                }

                return transaction.StateDiffV2(stateVector);
            }
        });
    }

    public async ValueTask<UpdateResult> ApplyUpdateAsync(string name, byte[] stateDiff, object metadata)
    {
        var context = contexts.GetContext(name);

        return await context.ApplyUpdateReturnAsync(doc =>
        {
            var result = new UpdateResult
            {
                Update = stateDiff
            };

            using (var transaction = doc.WriteTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be created.");
                }

                result.TransactionUpdateResult = transaction.ApplyV2(stateDiff);
            }

            return result;
        });
    }

    public async ValueTask UpdateDocAsync(string name, Action<Doc, Transaction> action)
    {
        var context = contexts.GetContext(name);

        await context.ApplyUpdateReturnAsync(doc =>
        {
            using (var transaction = doc.WriteTransaction())
            {
                if (transaction == null)
                {
                    throw new InvalidOperationException("Transaction cannot be created.");
                }

                action(doc, transaction);
            }

            return true;
        });
    }
}
