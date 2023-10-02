using YDotNet.Protocol;

namespace YDotNet.Server.WebSockets;

public static class EncoderExtensions
{
    public static async Task WriteSyncStep1Async(this Encoder encoder, byte[] stateVector,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct);
        await encoder.WriteVarUintAsync(MessageTypes.SyncStep1, ct);
        await encoder.WriteVarUint8Array(stateVector, ct);
    }

    public static async Task WriteSyncStep2Async(this Encoder encoder, byte[] update,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct);
        await encoder.WriteVarUintAsync(MessageTypes.SyncStep2, ct);
        await encoder.WriteVarUint8Array(update, ct);
    }

    public static async Task WriteSyncUpdateAsync(this Encoder encoder, byte[] update,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct);
        await encoder.WriteVarUintAsync(MessageTypes.SyncUpdate, ct);
        await encoder.WriteVarUint8Array(update, ct);
    }

    public static async Task WriteAwarenessAsync(this Encoder encoder, long clientId, long clock, string? state,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync((int)clientId, ct);
        await encoder.WriteVarUintAsync((int)clock, ct);
        await encoder.WriteVarStringAsync(state ?? string.Empty, ct);
    }
}
