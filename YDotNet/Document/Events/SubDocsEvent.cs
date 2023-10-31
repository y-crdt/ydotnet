using YDotNet.Native.Document.Events;

namespace YDotNet.Document.Events;

/// <summary>
///     Event used to communicate the load requests from the underlying sub-documents.
/// </summary>
public class SubDocsEvent
{
    internal SubDocsEvent(SubDocsEventNative native, Doc doc)
    {
        Added = native.Added().Select(h => doc.GetDoc(h, isDeleted: false)).ToList();
        Removed = native.Removed().Select(h => doc.GetDoc(h, isDeleted: true)).ToList();
        Loaded = native.Loaded().Select(h => doc.GetDoc(h, isDeleted: false)).ToList();
    }

    /// <summary>
    ///     Gets the sub-documents that were added to the <see cref="Doc" /> instance that emitted this event.
    /// </summary>
    public IReadOnlyList<Doc> Added { get; }

    /// <summary>
    ///     Gets the sub-documents that were removed to the <see cref="Doc" /> instance that emitted this event.
    /// </summary>
    public IReadOnlyList<Doc> Removed { get; }

    /// <summary>
    ///     Gets the sub-documents that were loaded to the <see cref="Doc" /> instance that emitted this event.
    /// </summary>
    public IReadOnlyList<Doc> Loaded { get; }
}
