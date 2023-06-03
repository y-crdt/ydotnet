namespace YDotNet.Document.Events;

/// <summary>
///     A clear event passed to a callback subscribed to <see cref="Doc.ObserveClear" />.
/// </summary>
public class ClearEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClearEvent" /> class.
    /// </summary>
    /// <param name="doc">The initial value for <see cref="Doc" />.</param>
    public ClearEvent(Doc doc)
    {
        Doc = doc;
    }

    /// <summary>
    ///     Gets the <see cref="Doc" /> associated with this event.
    /// </summary>
    public Doc Doc { get; }
}
