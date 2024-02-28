namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the type of data is held by the related <see cref="EventPathSegment" /> instance.
/// </summary>
public enum EventPathSegmentTag : sbyte
{
    /// <summary>
    ///     The <see cref="EventPathSegment" /> contains a <see cref="string" /> instance.
    /// </summary>
    Key = 1,

    /// <summary>
    ///     The <see cref="EventPathSegment" /> contains an <see cref="int" /> value.
    /// </summary>
    Index = 2,
}
