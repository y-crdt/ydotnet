namespace YDotNet.Server;

public sealed class CleanupOptions
{
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);

    public TimeSpan LogWaitTime { get; set; } = TimeSpan.FromMinutes(10);
}
