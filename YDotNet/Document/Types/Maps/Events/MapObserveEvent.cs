using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps.Events;

// TODO [LSViana] Check if this event should have a `*Native` counterpart like `Doc` events have.
//
// It wasn't created here because reading values of the event requires function calls instead of
// being passed through the FFI call as a `struct`. Then, executing all methods for every event may
// cause performance penalties instead of calling the methods on-demand as clients access it.

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Map" /> instance.
/// </summary>
public class MapObserveEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapObserveEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal MapObserveEvent(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the keys that changed within the <see cref="Map" /> instance and triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="MapEventKeyChange" /> for more information.</para>
    /// </remarks>
    public EventKeys Keys
    {
        get
        {
            var handle = MapChannel.ObserveEventKeys(Handle, out var length);

            return new EventKeys(handle, length);
        }
    }

    /// <summary>
    ///     Gets the <see cref="Map" /> instance that is related to this <see cref="MapObserveEvent" /> instance.
    /// </summary>
    public Map? Target => ReferenceAccessor.Access(new Map(MapChannel.ObserveEventTarget(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
