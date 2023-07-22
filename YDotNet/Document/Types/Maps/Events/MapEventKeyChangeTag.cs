namespace YDotNet.Document.Types.Maps.Events;

/// <summary>
///     Represents the tags to identify the kind of operation done within an <see cref="Map" /> under a certain key.
/// </summary>
public enum MapEventKeyChangeTag
{
    /// <summary>
    ///     Represents that the value under this key was added.
    /// </summary>
    Add,

    /// <summary>
    ///     Represents that the value under this key was removed.
    /// </summary>
    Remove,

    /// <summary>
    ///     Represents that the value under this key was updated.
    /// </summary>
    Update
}
