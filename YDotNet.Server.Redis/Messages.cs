using System.Text.Json.Serialization;

namespace YDotNet.Server.Redis;

public sealed class Message
{
    [JsonPropertyName("s")]
    required public Guid SenderId { get; init; }

    [JsonPropertyName("c")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PingMessage? Pinged { get; set; }

    [JsonPropertyName("d")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public PingMessage[]? ClientDisconnected { get; set; }

    [JsonPropertyName("u")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DocumentChangeMessage? DocumentChanged { get; set; }
}

public abstract class ClientMessage
{
    [JsonPropertyName("d")]
    required public string DocumentName { get; init; }

    [JsonPropertyName("c")]
    required public long ClientId { get; init; }
}

public sealed class DocumentChangeMessage : ClientMessage
{
    [JsonPropertyName("u")]
    required public byte[] DocumentDiff { get; init; }
}

public sealed class PingMessage : ClientMessage
{
    [JsonPropertyName("c")]
    required public long ClientClock { get; init; }

    [JsonPropertyName("s")]
    required public string? ClientState { get; init; }
}
