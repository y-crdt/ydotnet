namespace YDotNet.Server.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using YDotNet.Server.Storage;

internal class EFDocumentStorage<T>(
    IDbContextFactory<T> dbContextFactory,
    IOptions<EFDocumentStorageOptions> options)
    : IDocumentStorage
    where T : DbContext
{
    private readonly EFDocumentStorageOptions options = options.Value;

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    public async ValueTask<byte[]?> GetDocAsync(string name, CancellationToken ct = default)
    {
        var context = await dbContextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

        await using (context.ConfigureAwait(false))
        {
            var document = await context.Set<YDotNetDocument>().Where(x => x.Id == name)
                .FirstOrDefaultAsync(ct).ConfigureAwait(false);

            return document?.Data;
        }
    }

    public async ValueTask StoreDocAsync(string name, byte[] doc, CancellationToken ct = default)
    {
        var context = await dbContextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

        await using (context.ConfigureAwait(false))
        {
            var document = await context.Set<YDotNetDocument>().Where(x => x.Id == name)
                .FirstOrDefaultAsync(ct).ConfigureAwait(false);

            if (document != null)
            {
                document.Data = doc;
                document.Expiration = GetExpiration(name);
                context.Update(document);
            }
            else
            {
                document = new YDotNetDocument { Id = name, Data = doc, Expiration = GetExpiration(name) };
                await context.AddAsync(document, ct).ConfigureAwait(false);
            }

            await context.SaveChangesAsync(ct).ConfigureAwait(false);
        }
    }

    private DateTime? GetExpiration(string name)
    {
        var relative = options.Expiration?.Invoke(name);

        if (relative == null)
        {
            return null;
        }

        return Clock() + relative.Value;
    }
}
