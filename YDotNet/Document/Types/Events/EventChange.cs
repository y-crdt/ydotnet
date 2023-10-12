using YDotNet.Document.Cells;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a single change applied to a shared data type.
/// </summary>
public class EventChange
{
    internal EventChange(EventChangeNative native, Doc doc)
    {
        Length = native.Length;

        Tag = native.TagNative switch
        {
            EventChangeTagNative.Add => EventChangeTag.Add,
            EventChangeTagNative.Remove => EventChangeTag.Remove,
            EventChangeTagNative.Retain => EventChangeTag.Retain,
            _ => throw new NotSupportedException($"The value \"{native.TagNative}\" for {nameof(EventChangeTagNative)} is not supported."),
        };

        Values = native.ValuesHandles.Select(x => new Output(x, doc)).ToList();
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
    public IReadOnlyList<Output> Values { get; }
}
