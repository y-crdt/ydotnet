namespace YDotNet.Server.MongoDB;

public sealed class MongoDocumentStorageOptions
{
    public Func<string, TimeSpan?>? Expiration { get; set; }

    public string DatabaseName { get; set; } = "YDotNet";

    public string CollectionName { get; set; } = "YDotNet";
}
