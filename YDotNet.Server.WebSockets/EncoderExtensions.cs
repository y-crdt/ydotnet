using YDotNet.Protocol;

namespace YDotNet.Server.WebSockets;

public static class EncoderExtensions
{
    public static async Task WriteSyncStep1Async(this WebSocketEncoder encoder, byte[] stateVector, CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(MessageTypes.SyncStep1, ct).ConfigureAwait(false);
        await encoder.WriteVarUint8Array(stateVector, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    public static async Task WriteSyncStep2Async(this WebSocketEncoder encoder, byte[] update, CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(MessageTypes.SyncStep2, ct).ConfigureAwait(false);
        await encoder.WriteVarUint8Array(update, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    public static async Task WriteSyncUpdateAsync(this WebSocketEncoder encoder, byte[] update, CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeSync, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(MessageTypes.SyncUpdate, ct).ConfigureAwait(false);
        await encoder.WriteVarUint8Array(update, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    public static async Task WriteAuthErrorAsync(this WebSocketEncoder encoder, string reason, CancellationToken ct)
    {
        await encoder.WriteVarUintAsync(MessageTypes.TypeAuth, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(0, ct).ConfigureAwait(false);
        await encoder.WriteVarStringAsync(reason, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    public static async Task WriteAwarenessAsync(this WebSocketEncoder encoder, (ulong ClientId, ulong Clock, string? State)[] clients,
        CancellationToken ct)
    {
        if (clients.Length == 0)
        {
            return;
        }

        await encoder.WriteVarUintAsync(MessageTypes.TypeAwareness, ct).ConfigureAwait(false);

        var buffer = new BufferEncoder();

        await buffer.WriteVarUintAsync((ulong)clients.Length, ct).ConfigureAwait(false);

        foreach (var (clientId, clock, state) in clients)
        {
            await buffer.WriteVarUintAsync(clientId, ct).ConfigureAwait(false);
            await buffer.WriteVarUintAsync(clock, ct).ConfigureAwait(false);
            await buffer.WriteVarStringAsync(state ?? string.Empty, ct).ConfigureAwait(false);
        }

        await encoder.WriteVarUint8Array(buffer.ToArray(), ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }
}
