namespace YDotNet.Server;

public sealed class DocumentContext
{
    required public string DocumentName { get; set; }

    required public long ClientId { get; set; }

    public object? Metadata { get; set; }
}
