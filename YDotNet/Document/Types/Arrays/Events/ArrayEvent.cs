using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays.Events;

// TODO [LSViana] Check if this event should have a `*Native` counterpart like `Doc` events have (like MapEvent).

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Array" /> instance.
/// </summary>
public class ArrayEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ArrayEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal ArrayEvent(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="Array" /> instance.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventPath" /> for more information.</para>
    /// </remarks>
    public EventPath Path
    {
        get
        {
            var handle = ArrayChannel.ObserveEventPath(Handle, out var length);

            return new EventPath(handle, length);
        }
    }

    /// <summary>
    ///     Gets the changes within the <see cref="Array" /> instance and triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventChange" /> for more information.</para>
    /// </remarks>
    public EventChanges Delta
    {
        get
        {
            var handle = ArrayChannel.ObserveEventDelta(Handle, out var length);

            return new EventChanges(handle, length);
        }
    }

    /// <summary>
    ///     Gets the <see cref="Array" /> instance that is related to this <see cref="ArrayEvent" /> instance.
    /// </summary>
    public Array? Target => ReferenceAccessor.Access(new Array(ArrayChannel.ObserveEventTarget(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
