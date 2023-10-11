using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays.Events;

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Array" /> instance.
/// </summary>
public class ArrayEvent : UnmanagedResource
{
    private readonly Lazy<EventPath> path;
    private readonly Lazy<EventChanges> delta;
    private readonly Lazy<Array> target;

    internal ArrayEvent(nint handle)
        : base(handle)
    {
        path = new Lazy<EventPath>(() =>
        {
            var pathHandle = ArrayChannel.ObserveEventPath(handle, out var length).Checked();

            return new EventPath(pathHandle, length, this);
        });

        delta = new Lazy<EventChanges>(() =>
        {
            var deltaHandle = ArrayChannel.ObserveEventDelta(handle, out var length).Checked();

            return new EventChanges(deltaHandle, length, this);
        });

        target = new Lazy<Array>(() =>
        {
            var targetHandle = ArrayChannel.ObserveEventTarget(handle).Checked();

            return new Array(targetHandle);
        });
    }

    protected override void DisposeCore(bool disposing)
    {
        // The event has no explicit garbage collection, but is released automatically after the event has been completed.
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
    public EventChanges Delta => delta.Value;

    /// <summary>
    ///     Gets the <see cref="Array" /> instance that is related to this <see cref="ArrayEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    public Array Target => target.Value;
}
