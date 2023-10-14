using YDotNet.Document.Cells;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a single change made within the parent instance.
/// </summary>
/// <remarks>
///     Use the <see cref="Key" /> and <see cref="Tag" /> properties to identify where and the type of change
///     represented by this <see cref="EventKeyChange" /> instance.
/// </remarks>
public class EventKeyChange
{
    internal EventKeyChange(EventKeyChangeNative native, Doc doc)
    {
        Key = native.Key();

        Tag = native.TagNative switch
        {
            EventKeyChangeTagNative.Add => EventKeyChangeTag.Add,
            EventKeyChangeTagNative.Remove => EventKeyChangeTag.Remove,
            EventKeyChangeTagNative.Update => EventKeyChangeTag.Update,
            _ => throw new NotSupportedException($"The value \"{native.TagNative}\" for {nameof(EventKeyChangeTagNative)} is not supported."),
        };

        if (native.OldValue != nint.Zero)
        {
            OldValue = new Output(native.OldValue, doc, true);
        }

        if (native.NewValue != nint.Zero)
        {
            NewValue = new Output(native.NewValue, doc, false);
        }
    }

    /// <summary>
    ///     Gets the key of the value that changed in the parent instance.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the type of change that was performed in the <see cref="Key" /> entry.
    /// </summary>
    public EventKeyChangeTag Tag { get; }

    /// <summary>
    ///     Gets the old value that was present in the <see cref="Key" /> before the change.
    /// </summary>
    public Output? OldValue { get; }

    /// <summary>
    ///     Gets the new value that is present in the <see cref="Key" /> after the change.
    /// </summary>
    public Output? NewValue { get; }
}
