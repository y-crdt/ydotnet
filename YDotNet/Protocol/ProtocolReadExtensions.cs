namespace YDotNet.Protocol;

/// <summary>
/// Encoders extensions to implement the yjs protocol.
/// </summary>
public static class ProtocolReadExtensions
{
    /// <summary>
    /// Reads the next message from the coder.
    /// </summary>
    /// <param name="decoder">The decoder to read from.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The next message or <see cref="UnknownMessage"/> for unknown messages.
    /// </returns>
    public static async ValueTask<BaseMessage> ReadNextMessageAsync(this Decoder decoder, CancellationToken ct)
    {
        var messageType = await decoder.ReadVarUintAsync(ct).ConfigureAwait(false);

        switch (messageType)
        {
            case SyncMessage.BaseIdentifier:
                return await ReadSyncMessageAsync(decoder, ct).ConfigureAwait(false);

            case QueryAwarnessMessage.Identifier:
                return await ReadAwarenessMessageAsync(decoder, ct).ConfigureAwait(false);

            default:
                return new UnknownMessage(messageType);
        }
    }

    /// <summary>
    /// Reads the current sync message from the coder.
    /// </summary>
    /// <param name="decoder">The decoder to read from.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The sync message or <see cref="UnknownMessage"/> for unknown messages.
    /// </returns>
    /// <exception cref="InvalidOperationException">Protocol error occurred.</exception>
    public static async ValueTask<SyncMessage> ReadSyncMessageAsync(this Decoder decoder, CancellationToken ct)
    {
        var syncType = await decoder.ReadVarUintAsync(ct).ConfigureAwait(false);

        switch (syncType)
        {
            case SyncStep1Message.Identifier:
                var stateVector = await decoder.ReadVarUint8ArrayAsync(ct).ConfigureAwait(false);

                return new SyncStep1Message(stateVector);

            case SyncStep2Message.Identifier:
                var syncUpdate = await decoder.ReadVarUint8ArrayAsync(ct).ConfigureAwait(false);

                return new SyncStep2Message(syncUpdate);

            case SyncUpdateMessage.Identifier:
                var update = await decoder.ReadVarUint8ArrayAsync(ct).ConfigureAwait(false);

                return new SyncUpdateMessage(update);

            default:
                return new UnknownSyncMessage(syncType);
        }
    }

    /// <summary>
    /// Reads the current awareness message from the coder.
    /// </summary>
    /// <param name="decoder">The decoder to read from.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The awareness message.
    /// </returns>
    /// <exception cref="InvalidOperationException">Protocol error occurred.</exception>
    private static async Task<AwarenessMessage> ReadAwarenessMessageAsync(this Decoder decoder, CancellationToken ct)
    {
        // This is the length of the awareness message (for whatever reason).
        await decoder.ReadVarUintAsync(ct).ConfigureAwait(false);

        var clientCount = await decoder.ReadVarUintAsync(ct).ConfigureAwait(false);
        var clientList = new List<AwarenessInformation>();

        for (var i = 0ul; i < clientCount; i++)
        {
            var clientId = await decoder.ReadVarUintAsync(ct).ConfigureAwait(false);

            var clientClock = await decoder.ReadVarUintAsync(ct).ConfigureAwait(false);
            var clientState = await decoder.ReadVarStringAsync(ct).ConfigureAwait(false);

            clientList.Add(new AwarenessInformation(clientId, clientClock, clientState));
        }

        return new AwarenessMessage(clientList.ToArray());
    }
}
