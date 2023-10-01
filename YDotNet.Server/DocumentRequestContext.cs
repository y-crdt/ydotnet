namespace YDotNet.Server;

public sealed class DocumentContext
{
    required public string DocumentName { get; init; }

    required public long ClientId { get; init; }

    public object? Metadata { get; init; }
}
