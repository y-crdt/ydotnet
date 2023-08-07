using YDotNet.Document.Cells;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a single change applied to a shared data type.
/// </summary>
public class EventChange
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventChange" /> class.
    /// </summary>
    /// <param name="tag">The type of change represented by this instance.</param>
    /// <param name="length">The amount of elements affected by the current change.</param>
    /// <param name="values">
    ///     Optional, the values affected by the current change if <see cref="Tag" /> is
    ///     <see cref="EventChangeTag.Add" />.
    /// </param>
    public EventChange(EventChangeTag tag, uint length, IEnumerable<Output>? values)
    {
        Tag = tag;
        Length = length;
        Values = values;
    }

    /// <summary>
    ///     Gets the type of change represented by this instance.
    /// </summary>
    public EventChangeTag Tag { get; }

    /// <summary>
    ///     Gets the amount of changes represented by this instance.
    /// </summary>
    public uint Length { get; }

    /// <summary>
    ///     Gets the values that were affected by this change.
    /// </summary>
    public IEnumerable<Output>? Values { get; }
}
