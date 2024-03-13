namespace YDotNet.Server;

public sealed class DocumentManagerOptions
{
    public bool AutoCreateDocument { get; set; } = true;

    public TimeSpan StoreDebounce { get; set; } = TimeSpan.FromMilliseconds(1000);

    public TimeSpan MaxWriteTimeInterval { get; set; } = TimeSpan.FromSeconds(5);

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan MaxPingTime { get; set; } = TimeSpan.FromMinutes(1);
}
