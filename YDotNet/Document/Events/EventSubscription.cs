namespace YDotNet.Document.Events;

/// <summary>
///     Represents a subscription to an event of <see cref="Doc" />.
/// </summary>
public class EventSubscription
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventSubscription" /> class.
    /// </summary>
    /// <param name="id">The ID used to identify this instance of <see cref="EventSubscription" />.</param>
    /// <param name="callback">The callback to be invoked when the related event is triggered.</param>
    internal EventSubscription(uint id, Delegate callback)
    {
        Id = id;
        Callback = callback;
    }

    /// <summary>
    ///     Gets the ID used to identify this subscription in the document.
    /// </summary>
    public uint Id { get; }

    /// <summary>
    ///     Gets the callback to be invoked when the related event is triggered.
    /// </summary>
    /// <remarks>
    ///     A reference to the callback is stored to make sure it's not disposed by the GC before the unsubscription.
    /// </remarks>
    public Delegate Callback { get; }
}
