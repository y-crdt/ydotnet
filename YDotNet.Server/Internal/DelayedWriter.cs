namespace YDotNet.Server.Internal;

internal sealed class DelayedWriter
{
    private readonly TimeSpan delay;
    private readonly TimeSpan delayMax;
    private readonly Func<Task> action;
    private int pendingWrites = 0;
    private DateTime lastWrite;
    private Timer? writeTimer;
    private Task? writeTask;

    public Func<DateTime> Clock = () => DateTime.UtcNow;

    public DelayedWriter(TimeSpan delay, TimeSpan delayMax, Func<Task> action)
    {
        this.delay = delay;
        this.delayMax = delayMax;
        this.action = action;
    }

    public async Task FlushAsync()
    {
        writeTimer?.Dispose();

        if (writeTask != null)
        {
            await writeTask;
        }

        if (pendingWrites > 0)
        {
            Write();
        }

        if (writeTask != null)
        {
            await writeTask;
        }
    }

    public void Ping()
    {
        var now = Clock();

        Interlocked.Increment(ref pendingWrites);

        if (lastWrite == default)
        {
            lastWrite = now;
        }

        var timeSinceLastPing = now - lastWrite;
        if (timeSinceLastPing > delayMax)
        {
            Write();
        }
        else
        {
            writeTimer?.Dispose();
            writeTimer = new Timer(_ => Write(), null, (int)delay.TotalMilliseconds, 0);
        }
    }

    private void Write()
    {
        _ = WriteAsync();
    }

    private async Task WriteAsync()
    {
        var now = Clock();

        if (writeTask != null)
        {
            return;
        }

        var localPendingWrites = pendingWrites;

        var task = action();
        try
        {
            writeTask = task;
            await task;
        }
        finally
        {
            lastWrite = Clock();

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Interlocked.CompareExchange(ref writeTask, null, task);
            Interlocked.Add(ref pendingWrites, -localPendingWrites);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        }
    }
}
