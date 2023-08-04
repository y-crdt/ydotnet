using YDotNet.Document.Cells;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a single change applied to a shared data type.
/// </summary>
public class EventDelta
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventDelta" /> class.
    /// </summary>
    /// <param name="tag">The type of change represented by this instance.</param>
    /// <param name="length">The amount of changes represented by this instance.</param>
    /// <param name="insert">
    ///     The value that was added, this property is if <see cref="Tag" /> is
    ///     <see cref="EventDeltaTag.Add" />.
    /// </param>
    /// <param name="attributes">The attributes that are part of the changed content.</param>
    public EventDelta(EventDeltaTag tag, uint length, Output? insert, IEnumerable<EventDeltaAttribute> attributes)
    {
        Tag = tag;
        Length = length;
        Insert = insert;
        Attributes = attributes;
    }

    /// <summary>
    ///     Gets the type of change represented by this instance.
    /// </summary>
    public EventDeltaTag Tag { get; }

    /// <summary>
    ///     Gets the amount of changes represented by this instance.
    /// </summary>
    public uint Length { get; }

    /// <summary>
    ///     Gets the value that was added, this property is if <see cref="Tag" /> is <see cref="EventDeltaTag.Add" />.
    /// </summary>
    public Output? Insert { get; }

    /// <summary>
    ///     Gets the attributes that are part of the changed content.
    /// </summary>
    public IEnumerable<EventDeltaAttribute> Attributes { get; }
}
