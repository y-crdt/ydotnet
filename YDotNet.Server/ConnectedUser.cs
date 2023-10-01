namespace YDotNet.Server;

public sealed class ConnectedUser
{
    public Dictionary<string, object> State { get; set; } = new Dictionary<string, object>();

    public DateTime LastActivity { get; set; }
}
