namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the type of change represented by the parent <see cref="EventDelta" /> instance.
/// </summary>
public enum EventDeltaTag
{
    /// <summary>
    ///     Represents the addition of content.
    /// </summary>
    /// <remarks>
    ///     In this case, the value of <see cref="EventDelta" />.<see cref="EventDelta.Insert" /> will not be <c>null</c>.
    /// </remarks>
    Add,

    /// <summary>
    ///     Represents the removal of content.
    /// </summary>
    Remove,

    /// <summary>
    ///     Represents the update of content.
    /// </summary>
    Retain,
}
