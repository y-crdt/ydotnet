namespace YDotNet.Server.Clustering;

public sealed class RedisDocumentStorageOptions
{
    public Func<string, TimeSpan?>? Expiration { get; set; }

    public int Database { get; set; }

    public string Prefix { get; set; } = "YDotNetDocument_";
}
