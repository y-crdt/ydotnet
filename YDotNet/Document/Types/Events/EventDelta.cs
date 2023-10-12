using YDotNet.Document.Cells;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a single change applied to a shared data type.
/// </summary>
public class EventDelta
{
    internal EventDelta(EventDeltaNative native, Doc doc)
    {
        Length = native.Length;

        Tag = native.TagNative switch
        {
            EventDeltaTagNative.Add => EventDeltaTag.Add,
            EventDeltaTagNative.Remove => EventDeltaTag.Remove,
            EventDeltaTagNative.Retain => EventDeltaTag.Retain,
            _ => throw new NotSupportedException($"The value \"{native.TagNative}\" for {nameof(EventDeltaTagNative)} is not supported."),
        };

        Attributes = native.Attributes.ToDictionary(
            x => x.Value.Key(),
            x => new Output(x.Value.ValueHandle(x.Handle), doc));

        if (native.InsertHandle != nint.Zero)
        {
            Insert = new Output(native.InsertHandle, doc);
        }
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
    public IReadOnlyDictionary<string, Output> Attributes { get; }
}
