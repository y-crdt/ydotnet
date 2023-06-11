namespace YDotNet.Document.Events;

/// <summary>
///     An update event passed to a callback subscribed to <see cref="Doc.ObserveUpdatesV1" /> or
///     <see cref="Doc.ObserveUpdatesV2" />.
/// </summary>
public class UpdateEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateEvent" /> class.
    /// </summary>
    /// <param name="update">The binary information this event represents.</param>
    public UpdateEvent(byte[] update)
    {
        Update = update;
    }

    /// <summary>
    ///     Gets the binary information about all inserted and deleted changes performed within the scope of its transaction.
    /// </summary>
    public byte[] Update { get; }
}
