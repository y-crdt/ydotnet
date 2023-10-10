using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays.Events;

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Array" /> instance.
/// </summary>
public class ArrayEvent
{
    private readonly Lazy<EventPath> path;
    private readonly Lazy<EventChanges> delta;
    private readonly Lazy<Array> target;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ArrayEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal ArrayEvent(nint handle)
    {
        Handle = handle;

        path = new Lazy<EventPath>(() =>
        {
            var pathHandle = ArrayChannel.ObserveEventPath(handle, out var length).Checked();
            return new EventPath(pathHandle, length);
        });

        delta = new Lazy<EventChanges>(() =>
        {
            var deltaHandle = ArrayChannel.ObserveEventDelta(handle, out var length).Checked();
            return new EventChanges(deltaHandle, length);
        });

        target = new Lazy<Array>(() =>
        {
            var targetHandle = ArrayChannel.ObserveEventTarget(handle).Checked();
            return new Array(targetHandle);
        });
    }

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="Array" /> instance.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventPath Path => path.Value;

    /// <summary>
    ///     Gets the changes within the <see cref="Array" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventChanges Delta
    {
        get
        {
            var handle = ArrayChannel.ObserveEventDelta(Handle, out var length);

            return new EventChanges(handle, length);
        }
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the <see cref="Array" /> instance that is related to this <see cref="ArrayEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    /// <remarks>You are responsible to dispose the text, if you use this property.</remarks>
    public Array ResolveTarget() => target.Value;
}
