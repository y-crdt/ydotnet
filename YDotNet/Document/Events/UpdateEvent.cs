using YDotNet.Native.Document.Events;

namespace YDotNet.Document.Events;

/// <summary>
///     An update event passed to a callback subscribed to <see cref="Doc.ObserveUpdatesV1" /> or
///     <see cref="Doc.ObserveUpdatesV2" />.
/// </summary>
public class UpdateEvent
{
    internal UpdateEvent(UpdateEventNative native)
    {
        Update = native.Bytes();
    }

    /// <summary>
    ///     Gets the binary information about all inserted and deleted changes performed within the scope of its transaction.
    /// </summary>
    public byte[] Update { get; }
}
