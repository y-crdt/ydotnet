using ProtoBuf;
using YDotNet.Server.Clustering.Internal;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace YDotNet.Server.Clustering;

[ProtoContract]
public enum MessageType
{
    ClientPinged,
    ClientDisconnected,
    AwarenessRequested,
    Update,
    SyncStep1,
    SyncStep2,
}

[ProtoContract]
public sealed class Message : ICanEstimateSize
{
    private static readonly int GuidLength = Guid.Empty.ToString().Length;

    [ProtoMember(1)]
    public MessageType Type { get; set; }

    [ProtoMember(1)]
    public Guid SenderId { get; set; }

    [ProtoMember(2)]
    public string DocumentName { get; set; }

    [ProtoMember(3)]
    public ulong ClientId { get; set; }

    [ProtoMember(4)]
    public ulong ClientClock { get; set; }

    [ProtoMember(5)]
    public string? ClientState { get; set; }

    [ProtoMember(6)]
    public byte[]? Data { get; set; }

    public int EstimateSize()
    {
        var size =
            GuidLength +
            sizeof(long) +
            sizeof(long) +
            DocumentName.Length +
            Data?.Length ?? 0;

        return size;
    }
}
