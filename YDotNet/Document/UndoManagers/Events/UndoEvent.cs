using YDotNet.Document.State;

namespace YDotNet.Document.UndoManagers.Events;

/// <summary>
///     Represents a redo/undo event from an <see cref="UndoManager" />.
/// </summary>
public class UndoEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UndoEvent" /> class.
    /// </summary>
    /// <param name="kind">The kind of the event.</param>
    /// <param name="origin">The origin of the event.</param>
    /// <param name="insertions">The <see cref="DeleteSet" /> entries for inserted content.</param>
    /// <param name="deletions">The <see cref="DeleteSet" /> entries for deleted content.</param>
    internal UndoEvent(UndoEventKind kind, byte[] origin, DeleteSet insertions, DeleteSet deletions)
    {
        Kind = kind;
        Origin = origin;
        Insertions = insertions;
        Deletions = deletions;
    }

    /// <summary>
    ///     Gets the kind of the event.
    /// </summary>
    public UndoEventKind Kind { get; }

    /// <summary>
    ///     Gets the origin of the event.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Origin" /> is a binary marker.
    /// </remarks>
    public byte[] Origin { get; }

    /// <summary>
    ///     Gets the <see cref="DeleteSet" /> entries for inserted content.
    /// </summary>
    public DeleteSet Insertions { get; }

    /// <summary>
    ///     Gets the <see cref="DeleteSet" /> entries for deleted content.
    /// </summary>
    public DeleteSet Deletions { get; }
}
