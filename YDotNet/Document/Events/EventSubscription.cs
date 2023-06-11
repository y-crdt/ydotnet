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
    internal EventSubscription(uint id)
    {
        Id = id;
    }

    /// <summary>
    ///     Gets the ID used to identify this subscription in the document.
    /// </summary>
    public uint Id { get; }
}
