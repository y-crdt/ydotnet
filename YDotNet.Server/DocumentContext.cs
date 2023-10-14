namespace YDotNet.Server;

public sealed record DocumentContext(string DocumentName, long ClientId)
{
    public object? Metadata { get; set; }
}
