using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps.Events;

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Map" /> instance.
/// </summary>
public class MapEvent : UnmanagedResource
{
    private readonly Lazy<EventPath> path;
    private readonly Lazy<EventKeys> keys;
    private readonly Lazy<Map> target;

    internal MapEvent(nint handle, Doc doc)
        : base(handle)
    {
        path = new Lazy<EventPath>(() =>
        {
            var pathHandle = ArrayChannel.ObserveEventPath(handle, out var length).Checked();

            return new EventPath(pathHandle, length);
        });

        keys = new Lazy<EventKeys>(() =>
        {
            var keysHandle = MapChannel.ObserveEventKeys(handle, out var length).Checked();

            return new EventKeys(keysHandle, length, doc);
        });

        target = new Lazy<Map>(() =>
        {
            var targetHandle = MapChannel.ObserveEventTarget(handle).Checked();

            return doc.GetMap(targetHandle);
        });
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        // The event has no explicit garbage collection, but is released automatically after the event has been completed.
    }

    /// <summary>
    ///     Gets the keys that changed within the <see cref="Map" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventKeys Keys => keys.Value;

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="Map" /> instance.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventPath Path => path.Value;

    /// <summary>
    ///     Gets the <see cref="Map" /> instance that is related to this <see cref="MapEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    public Map Target => target.Value;
}
