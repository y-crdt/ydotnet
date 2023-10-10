using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps.Events;

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Map" /> instance.
/// </summary>
public class MapEvent
{
    private readonly Lazy<EventPath> path;
    private readonly Lazy<EventKeys> keys;
    private readonly Lazy<Map> target;

    /// <summary>
    ///     Initializes a new instance of the <see cref="MapEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal MapEvent(nint handle)
    {
        Handle = handle;

        path = new Lazy<EventPath>(() =>
        {
            var pathHandle = ArrayChannel.ObserveEventPath(handle, out var length).Checked();
            return new EventPath(pathHandle, length);
        });

        keys = new Lazy<EventKeys>(() =>
        {
            var keysHandle = MapChannel.ObserveEventKeys(handle, out var length).Checked();
            return new EventKeys(keysHandle, length);
        });

        target = new Lazy<Map>(() =>
        {
            var targetHandle = MapChannel.ObserveEventTarget(handle).Checked();
            return new Map(targetHandle);
        });
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
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the <see cref="Map" /> instance that is related to this <see cref="MapEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    /// <remarks>You are responsible to dispose the text, if you use this property.</remarks>
    public Map ResolveTarget() => target.Value;
}
