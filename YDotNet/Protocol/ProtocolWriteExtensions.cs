namespace YDotNet.Protocol;

/// <summary>
/// Encoders extensions to implement the yjs protocol.
/// </summary>
public static class ProtocolWriteExtensions
{
    /// <summary>
    /// Writes the state vector to the encoder to implement Step 1 of the synchronization protocol.
    /// </summary>
    /// <param name="encoder">The encoder to write to.</param>
    /// <param name="message">The message with the state vector. Cannot be null.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public static async Task WriteAsync(this Encoder encoder, SyncStep1Message message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await encoder.WriteVarUintAsync(SyncMessage.BaseIdentifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(SyncStep1Message.Identifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUint8Array(message.StateVector, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the update to the encoder to implement Step 2 of the synchronization protocol.
    /// </summary>
    /// <param name="encoder">The encoder to write to.</param>
    /// <param name="message">The message with the update. Cannot be null.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public static async Task WriteAsync(this Encoder encoder, SyncStep2Message message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await encoder.WriteVarUintAsync(SyncMessage.BaseIdentifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(SyncStep1Message.Identifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUint8Array(message.Update, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes the incremental update to the encoder to implement Update Step of the synchronization protocol.
    /// </summary>
    /// <param name="encoder">The encoder to write to.</param>
    /// <param name="message">The message with the update. Cannot be null.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public static async Task WriteAsync(this Encoder encoder, SyncUpdateMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await encoder.WriteVarUintAsync(SyncMessage.BaseIdentifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(SyncUpdateMessage.Identifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUint8Array(message.Update, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Write an auth error to the encoder to notify the client about the authentication error.
    /// </summary>
    /// <param name="encoder">The encoder to write to.</param>
    /// <param name="message">The message with the reason why the authentication failed.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public static async Task WriteAsync(this Encoder encoder, AuthErrorMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await encoder.WriteVarUintAsync(AuthErrorMessage.Identifier, ct).ConfigureAwait(false);
        await encoder.WriteVarUintAsync(0, ct).ConfigureAwait(false);
        await encoder.WriteVarStringAsync(message.Reason, ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Write an message to the encoder to ask for awareness information.
    /// </summary>
    /// <param name="encoder">The encoder to write to.</param>
    /// <param name="message">The message.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public static async Task WriteAsync(this Encoder encoder, QueryAwarnessMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await encoder.WriteVarUintAsync(QueryAwarnessMessage.Identifier, ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Write the awareness information to the encoder to update the client with the current clients.
    /// </summary>
    /// <param name="encoder">The encoder to write to.</param>
    /// <param name="message">The message with the connected clients.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="message"/> is null.</exception>
    public static async Task WriteAsync(this Encoder encoder, AwarenessMessage message, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(message);

        await encoder.WriteVarUintAsync(AwarenessMessage.Identifier, ct).ConfigureAwait(false);

        var buffer = new BufferEncoder();

        await buffer.WriteVarUintAsync((ulong)message.Clients.Length, ct).ConfigureAwait(false);

        foreach (var (clientId, clock, state) in message.Clients)
        {
            await buffer.WriteVarUintAsync(clientId, ct).ConfigureAwait(false);
            await buffer.WriteVarUintAsync(clock, ct).ConfigureAwait(false);
            await buffer.WriteVarStringAsync(state ?? string.Empty, ct).ConfigureAwait(false);
        }

        await encoder.WriteVarUint8Array(buffer.ToArray(), ct).ConfigureAwait(false);
        await encoder.FlushAsync(ct).ConfigureAwait(false);
    }
}
