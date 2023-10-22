using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlTexts.Events;

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlText" /> instance.
/// </summary>
public class XmlTextEvent : UnmanagedResource
{
    private readonly Lazy<EventDeltas> delta;
    private readonly Lazy<EventKeys> keys;
    private readonly Lazy<XmlText> target;

    internal XmlTextEvent(nint handle, Doc doc)
        : base(handle)
    {
        delta = new Lazy<EventDeltas>(
            () =>
            {
                var deltaHandle = XmlTextChannel.ObserveEventDelta(handle, out var length).Checked();

                return new EventDeltas(deltaHandle, length, doc);
            });

        keys = new Lazy<EventKeys>(
            () =>
            {
                var keysHandle = XmlTextChannel.ObserveEventKeys(handle, out var length).Checked();

                return new EventKeys(keysHandle, length, doc);
            });

        target = new Lazy<XmlText>(
            () =>
            {
                var targetHandle = XmlTextChannel.ObserveEventTarget(handle).Checked();

                return doc.GetXmlText(handle, isDeleted: false);
            });
    }

    /// <summary>
    ///     Gets the changes that triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventDeltas Delta => delta.Value;

    /// <summary>
    ///     Gets the attributes that changed and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventKeys Keys => keys.Value;

    /// <summary>
    ///     Gets the <see cref="XmlText" /> instance that is related to this <see cref="XmlTextEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    public XmlText Target => target.Value;

    /// <inheritdoc />
    protected internal override void DisposeCore(bool disposing)
    {
        // The event has no explicit garbage collection, but is released automatically after the event has been completed.
    }
}
