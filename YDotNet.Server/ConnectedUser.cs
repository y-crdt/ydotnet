namespace YDotNet.Server;

public sealed class ConnectedUser
{
    public string? ClientState { get; set; }

    public ulong ClientClock { get; set; }

    public DateTime LastActivity { get; set; }
}
