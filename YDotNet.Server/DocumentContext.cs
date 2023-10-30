namespace YDotNet.Server;

public sealed record DocumentContext(string DocumentName, ulong ClientId)
{
    public object? Metadata { get; set; }
}
