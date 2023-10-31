using YDotNet.Protocol;

namespace YDotNet.Server.WebSockets;

public static class EncoderExtensions
{
    public static async Task WriteSyncStep1Async(this WebSocketEncoder encoder, byte[] stateVector,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct);
        await encoder.WriteVarUintAsync(MessageTypes.SyncStep1, ct);
        await encoder.WriteVarUint8Array(stateVector, ct);
        await encoder.FlushAsync(ct);
    }

    public static async Task WriteSyncStep2Async(this WebSocketEncoder encoder, byte[] update,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct);
        await encoder.WriteVarUintAsync(MessageTypes.SyncStep2, ct);
        await encoder.WriteVarUint8Array(update, ct);
        await encoder.FlushAsync(ct);
    }

    public static async Task WriteSyncUpdateAsync(this WebSocketEncoder encoder, byte[] update,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct);
        await encoder.WriteVarUintAsync(MessageTypes.SyncUpdate, ct);
        await encoder.WriteVarUint8Array(update, ct);
        await encoder.FlushAsync(ct);
    }

    public static async Task WriteAuthErrorAsync(this WebSocketEncoder encoder, string reason,
        CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeAuth, ct);
        await encoder.WriteVarUintAsync(0, ct);
        await encoder.WriteVarStringAsync(reason, ct);
        await encoder.FlushAsync(ct);
    }

    public static async Task WriteAwarenessAsync(this WebSocketEncoder encoder, (ulong ClientId, ulong Clock, string? State)[] clients,
        CancellationToken ct)
    {
        if (clients.Length == 0)
        {
            return;
        }

        await encoder.WriteVarUintAsync(MessageTypes.TypeAwareness, ct);

        var buffer = new BufferEncoder();

        await buffer.WriteVarUintAsync((ulong)clients.Length, ct);

        foreach (var (clientId, clock, state) in clients)
        {
            await buffer.WriteVarUintAsync(clientId, ct);
            await buffer.WriteVarUintAsync(clock, ct);
            await buffer.WriteVarStringAsync(state ?? string.Empty, ct);
        }

        await encoder.WriteVarUint8Array(buffer.ToArray(), ct);
        await encoder.FlushAsync(ct);
    }
}
