using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements.Events;

// TODO [LSViana] Check if this event should have a `*Native` counterpart like `Doc` events have (like MapEvent).

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlElement" /> instance.
/// </summary>
public class XmlElementEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlElementEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlElementEvent(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the changes within the <see cref="XmlElement" /> instance and triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventChange" /> for more information.</para>
    /// </remarks>
    public EventChanges Delta
    {
        get
        {
            var handle = XmlElementChannel.ObserveEventDelta(Handle, out var length);

            return new EventChanges(handle, length);
        }
    }

    /// <summary>
    ///     Gets the attributes that changed within the <see cref="XmlElement" /> instance and triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventKeys" /> for more information.</para>
    /// </remarks>
    public EventKeys Keys
    {
        get
        {
            var handle = XmlElementChannel.ObserveEventKeys(Handle, out var length);

            return new EventKeys(handle, length);
        }
    }

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> instance that is related to this <see cref="XmlElementEvent" /> instance.
    /// </summary>
    public XmlElement? Target => ReferenceAccessor.Access(new XmlElement(XmlElementChannel.ObserveEventTarget(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
