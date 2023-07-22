using YDotNet.Document.Cells;

namespace YDotNet.Document.Types.Maps.Events;

/// <summary>
///     Represents a single change made within a <see cref="Map" /> instance.
/// </summary>
/// <remarks>
///     Use the <see cref="Key" /> and <see cref="Tag" /> properties to identify where and the type of change
///     represented by this <see cref="MapEventKeyChange" /> instance.
/// </remarks>
public class MapEventKeyChange
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapEventKeyChange" /> class.
    /// </summary>
    /// <param name="key">The key of the value that changed in the <see cref="Map" /> instance.</param>
    /// <param name="tag">The type of change that was performed in the <see cref="Key" /> entry.</param>
    /// <param name="oldValue">The old value that was present in the <see cref="Key" /> before the change.</param>
    /// <param name="newValue">The new value that is present in the <see cref="Key" /> after the change.</param>
    internal MapEventKeyChange(string key, MapEventKeyChangeTag tag, Output? oldValue, Output? newValue)
    {
        Key = key;
        Tag = tag;
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    ///     Gets the key of the value that changed in the <see cref="Map" /> instance.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the type of change that was performed in the <see cref="Key" /> entry.
    /// </summary>
    public MapEventKeyChangeTag Tag { get; }

    /// <summary>
    ///     Gets the old value that was present in the <see cref="Key" /> before the change.
    /// </summary>
    public Output? OldValue { get; }

    /// <summary>
    ///     Gets the new value that is present in the <see cref="Key" /> after the change.
    /// </summary>
    public Output? NewValue { get; }
}
