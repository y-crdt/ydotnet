using Microsoft.Extensions.Options;
using YDotNet.Document;
using YDotNet.Server.Storage;

namespace YDotNet.Server;

public sealed class DefaultDocumentManager : IDocumentManager
{
    private readonly IDocumentStorage documentStorage;
    private readonly DocumentManagerOptions options;

    public DefaultDocumentManager(IDocumentStorage documentStorage,
        IOptions<DocumentManagerOptions> options)
    {
        this.documentStorage = documentStorage;
        this.options = options.Value;
    }

    public ValueTask<Doc?> GetDocAsync(string name,
        CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<UpdateResult> ApplyUpdateAsync(string name, byte[] stateDiff, object metadata,
        CancellationToken ct)
    {
        var doc = await documentStorage.GetDocAsync(name, ct);

        if (doc == null)
        {
            if (options.AutoCreateDocument)
            {
                doc = new Doc();
            }
            else
            {
                return new UpdateResult
                {
                    IsSkipped = true
                };
            }
        }

        var result = new UpdateResult();

        using (var transaction = doc.WriteTransaction())
        {
            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction cannot be created.");
            }

            result.TransactionUpdateResult = transaction.ApplyV2(stateDiff);
            transaction.Commit();

            result.Update = stateDiff;
        }

        return result;
    }
}
