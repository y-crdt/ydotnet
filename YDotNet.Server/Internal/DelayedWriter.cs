namespace YDotNet.Server.Internal;

internal sealed class DelayedWriter
{
    private readonly TimeSpan delay;
    private readonly TimeSpan delayMax;
    private readonly Func<Task> action;
    private readonly Timer writeTimer;
    private int pendingWrites = 0;
    private DateTime lastWrite;
    private Task? writeTask;

    public Func<DateTime> Clock = () => DateTime.UtcNow;

    public DelayedWriter(TimeSpan delay, TimeSpan delayMax, Func<Task> action)
    {
        this.delay = delay;
        this.delayMax = delayMax;
        this.action = action;

        writeTimer = new Timer(_ => Write(), null, Timeout.Infinite, Timeout.Infinite);
    }

    public async Task FlushAsync()
    {
        writeTimer.Change(Timeout.Infinite, Timeout.Infinite);

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
            // Trigger the write operation immediately, but use no in the current thread to avoid blocking.
            writeTimer.Change(0, Timeout.Infinite);
        }
        else
        {
            // Reset the timer with every change.
            writeTimer.Change((int)delay.TotalMilliseconds, Timeout.Infinite);
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
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Interlocked.Add(ref pendingWrites, -localPendingWrites);
        }
    }
}
