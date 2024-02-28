namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the tags to identify the kind of operation done within the parent instance under a certain key.
/// </summary>
public enum EventKeyChangeTag
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
    Update,
}
