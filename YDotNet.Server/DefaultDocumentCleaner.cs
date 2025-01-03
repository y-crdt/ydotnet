using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YDotNet.Server;

internal class DefaultDocumentCleaner(IDocumentManager documentManager, IOptions<CleanupOptions> options, ILogger<DefaultDocumentCleaner> logger) : BackgroundService
{
    private readonly CleanupOptions options = options.Value;
    private DateTime lastLogging;

    public Func<DateTime> Clock { get; } = () => DateTime.UtcNow;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(options.Interval);

        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                await documentManager.CleanupAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // This is an expected exception when the process stops. There is no good reason to handle that.
            }
            catch (Exception ex)
            {
                var now = Clock();

                var timeSinceLastLogging = now - lastLogging;

                // We run this loop very often. If there is an exception, it could flood the log with duplicate log entries.
                if (timeSinceLastLogging < options.LogWaitTime)
                {
                    // Therefore use a wait time between two log calls.
                    return;
                }

                logger.LogError(ex, "Failed to cleanup document manager");
                lastLogging = now;
            }
        }
    }
}
