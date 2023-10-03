namespace YDotNet.Server.MongoDB;

internal sealed class DocumentEntity
{
    required public string Id { get; set; }

    required public byte[] Data { get; set; }

    public DateTime? Expiration { get; private set; }
}
