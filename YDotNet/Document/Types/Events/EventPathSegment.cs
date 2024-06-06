using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a segment of the full path represented by <see cref="EventPath" />.
/// </summary>
public class EventPathSegment
{
    internal EventPathSegment(EventPathSegmentNative native)
    {
        Tag = (EventPathSegmentTag)native.Tag;

        switch (Tag)
        {
            case EventPathSegmentTag.Key:
                Key = native.Key();
                break;

            case EventPathSegmentTag.Index:
                Index = (uint)native.KeyOrIndex;
                break;
        }
    }

    /// <summary>
    ///     Gets the value that indicates the kind of data held by this <see cref="EventPathSegment" /> instance.
    /// </summary>
    public EventPathSegmentTag Tag { get; }

    /// <summary>
    ///     Gets the <see cref="string" /> key, if <see cref="Tag" /> is <see cref="EventPathSegmentTag.Key" />, or <c>null</c>
    ///     otherwise.
    /// </summary>
    public string? Key { get; }

    /// <summary>
    ///     Gets the <see ref="uint" /> index, if <see cref="Tag" /> is <see cref="EventPathSegmentTag.Index" />, or
    ///     <c>null</c> otherwise.
    /// </summary>
    public uint? Index { get; }
}
