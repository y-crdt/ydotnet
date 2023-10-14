namespace YDotNet.Server.Clustering;

public sealed class ClusteringOptions
{
    public TimeSpan DebounceTime { get; set; } = TimeSpan.FromMilliseconds(500);

    public int MaxBatchCount { get; set; } = 100;

    public int MaxBatchSize { get; set; } = 1024 * 1024;
}
