namespace YDotNet.Server;

public sealed class ConnectedUser
{
    public string? ClientState { get; set; }

    public long ClientClock { get; set; }

    public DateTime LastActivity { get; set; }
}
