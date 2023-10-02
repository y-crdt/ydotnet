namespace YDotNet.Server;

public sealed class ConnectedUser
{
    required public string? ClientState { get; set; }

    required public long ClientClock { get; set; }

    public DateTime LastActivity { get; set; }
}
