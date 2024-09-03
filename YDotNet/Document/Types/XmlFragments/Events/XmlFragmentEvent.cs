using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlFragments.Events;

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlFragment" /> instance.
/// </summary>
public class XmlFragmentEvent : UnmanagedResource
{
    private readonly Lazy<EventChanges> delta;
    private readonly Lazy<EventPath> path;
    private readonly Lazy<XmlFragment> target;

    internal XmlFragmentEvent(nint handle, Doc doc)
        : base(handle)
    {
        path = new Lazy<EventPath>(
            () =>
            {
                var pathHandle = XmlElementChannel.ObserveEventPath(handle, out var length).Checked();

                return new EventPath(pathHandle, length);
            });

        delta = new Lazy<EventChanges>(
            () =>
            {
                var deltaHandle = XmlElementChannel.ObserveEventDelta(handle, out var length).Checked();

                return new EventChanges(deltaHandle, length, doc);
            });

        target = new Lazy<XmlFragment>(
            () =>
            {
                var targetHandle = XmlElementChannel.ObserveEventTarget(handle).Checked();

                return doc.GetXmlFragment(targetHandle, isDeleted: false);
            });
    }

    /// <summary>
    ///     Gets the changes within the <see cref="XmlFragment" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventChanges Delta => delta.Value;

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="XmlFragment" /> instance.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventPath Path => path.Value;

    /// <summary>
    ///     Gets the <see cref="XmlFragment" /> instance that is
    ///     related to this <see cref="XmlFragmentEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    public XmlFragment Target => target.Value;

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        // The event has no explicit garbage collection, but is released automatically after the event has been completed.
    }
}
