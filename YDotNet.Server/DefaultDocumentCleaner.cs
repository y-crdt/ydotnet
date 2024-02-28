using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YDotNet.Server;

internal class DefaultDocumentCleaner : BackgroundService
{
    private readonly IDocumentManager documentManager;
    private readonly ILogger<DefaultDocumentCleaner> logger;
    private readonly CleanupOptions options;
    private DateTime lastLogging;

    public Func<DateTime> Clock { get; } = () => DateTime.UtcNow;

    public DefaultDocumentCleaner(IDocumentManager documentManager, IOptions<CleanupOptions> options, ILogger<DefaultDocumentCleaner> logger)
    {
        this.documentManager = documentManager;
        this.logger = logger;
        this.options = options.Value;
    }

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
                // Therefore use a wait time between two log calls.
                if (timeSinceLastLogging < options.LogWaitTime)
                {
                    return;
                }

                logger.LogError(ex, "Failed to cleanup document manager.");
                lastLogging = now;
            }
        }
    }
}
