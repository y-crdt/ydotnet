using ProtoBuf;
using YDotNet.Server.Redis.Internal;

namespace YDotNet.Server.Redis;

[ProtoContract]
public sealed class Message : ICanEstimateSize
{
    private static readonly int GuidLength = Guid.Empty.ToString().Length;

    [ProtoMember(1)]
    required public Guid SenderId { get; init; }

    [ProtoMember(2)]
    required public string DocumentName { get; init; }

    [ProtoMember(3)]
    required public long ClientId { get; init; }

    [ProtoMember(4)]
    public ClientPingMessage? ClientPinged { get; set; }

    [ProtoMember(5)]
    public ClientDisconnectMessage? ClientDisconnected { get; set; }

    [ProtoMember(6)]
    public DocumentChangeMessage? DocumentChanged { get; set; }

    public int EstimateSize()
    {
        var size =
            GuidLength +
            sizeof(long) +
            DocumentName.Length +
            ClientPinged?.EstimatedSize() ?? 0 +
            ClientDisconnected?.EstimatedSize() ?? 0 +
            DocumentChanged?.EstimatedSize() ?? 0;

        return size;
    }
}

[ProtoContract]
public sealed class ClientDisconnectMessage
{
    public int EstimatedSize() => 0;
}

[ProtoContract]
public sealed class DocumentChangeMessage
{
    [ProtoMember(1)]
    required public byte[] DocumentDiff { get; init; }

    public int EstimatedSize() => sizeof(long) + DocumentDiff?.Length ?? 0;
}

[ProtoContract]
public sealed class ClientPingMessage
{
    [ProtoMember(1)]
    required public long ClientClock { get; init; }

    [ProtoMember(2)]
    required public string? ClientState { get; init; }

    public int EstimatedSize() => sizeof(long) + ClientState?.Length ?? 0;
}
