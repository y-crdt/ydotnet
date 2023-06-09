namespace YDotNet.Document.Events;

/// <summary>
///     Event used to communicate the load requests from the underlying sub-documents.
/// </summary>
public class SubDocsEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="SubDocsEvent" /> class.
    /// </summary>
    /// <param name="added">The initial value for <see cref="Added" />.</param>
    /// <param name="removed">The initial value for <see cref="Removed" />.</param>
    /// <param name="loaded">The initial value for <see cref="Loaded" />.</param>
    public SubDocsEvent(Doc[] added, Doc[] removed, Doc[] loaded)
    {
        Added = added;
        Removed = removed;
        Loaded = loaded;
    }

    /// <summary>
    ///     Gets the sub-documents that were added to the <see cref="Doc" /> instance that emitted this event.
    /// </summary>
    public Doc[] Added { get; }


    /// <summary>
    ///     Gets the sub-documents that were removed to the <see cref="Doc" /> instance that emitted this event.
    /// </summary>
    public Doc[] Removed { get; }


    /// <summary>
    ///     Gets the sub-documents that were loaded to the <see cref="Doc" /> instance that emitted this event.
    /// </summary>
    public Doc[] Loaded { get; }
}
