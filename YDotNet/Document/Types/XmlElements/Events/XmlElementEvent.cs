using YDotNet.Document.Types.Events;
using YDotNet.Document.Types.XmlFragments.Events;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements.Events;

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlElement" /> instance.
/// </summary>
public class XmlElementEvent : XmlFragmentEvent
{
    private readonly Lazy<EventKeys> keys;
    private readonly Lazy<XmlElement> target;

    internal XmlElementEvent(nint handle, Doc doc)
        : base(handle, doc)
    {
        keys = new Lazy<EventKeys>(
            () =>
            {
                var keysHandle = XmlElementChannel.ObserveEventKeys(handle, out var length).Checked();

                return new EventKeys(keysHandle, length, doc);
            });

        target = new Lazy<XmlElement>(
            () =>
            {
                var targetHandle = XmlElementChannel.ObserveEventTarget(handle).Checked();

                return doc.GetXmlElement(targetHandle, isDeleted: false);
            });
    }

    /// <summary>
    ///     Gets the attributes that changed within the <see cref="XmlElement" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventKeys Keys => keys.Value;

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> instance that is related to this <see cref="XmlElementEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    public new XmlElement Target => target.Value;
}
