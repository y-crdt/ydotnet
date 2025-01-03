namespace YDotNet.Server.EntityFramework;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public sealed class EFDocumentCleaner(
    IDbContextFactory<YDotNetContext> dbContextFactory,
    ILogger<EFDocumentCleaner> logger,
    IOptions<EFDocumentStorageOptions> options)
    : BackgroundService
{
    private readonly EFDocumentStorageOptions options = options.Value;

    public Func<DateTime> Clock { get; set; } = () => DateTime.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var interval = options.CleanupInterval;
        if (interval <= TimeSpan.Zero || interval == TimeSpan.MaxValue)
        {
            logger.LogDebug("Not running cleaner, disabled by interval");
            return;
        }

        var timer = new PeriodicTimer(interval);

        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                await CleanupAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // This is an expected exception when the process stops. There is no good reason to handle that.
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to cleanup storage");
            }
        }
    }

    public async Task CleanupAsync(CancellationToken ct)
    {
        var context = await dbContextFactory.CreateDbContextAsync(ct).ConfigureAwait(false);

        await using (context.ConfigureAwait(false))
        {
            var now = Clock();
            var deletion = context.Documents.Where(x => x.Expiration < now);

            var deleted = await deletion.ExecuteDeleteAsync(ct).ConfigureAwait(false);
            if (deleted > 0)
            {
                logger.LogInformation("{items} cleaned", deleted);
            }
            else
            {
                logger.LogDebug("No items cleaned");
            }
        }
    }
}
