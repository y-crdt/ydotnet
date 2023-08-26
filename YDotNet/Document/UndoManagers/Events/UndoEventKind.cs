namespace YDotNet.Document.UndoManagers.Events;

/// <summary>
///     Represents the kind of change in an <see cref="UndoEvent" />.
/// </summary>
public enum UndoEventKind
{
    /// <summary>
    ///     Represents an undo operation.
    /// </summary>
    Undo = 0,

    /// <summary>
    ///     Represents a redo operation.
    /// </summary>
    Redo = 1
}
