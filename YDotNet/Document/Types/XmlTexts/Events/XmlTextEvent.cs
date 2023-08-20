using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlTexts.Events;

// TODO [LSViana] Check if this event should have a `*Native` counterpart like `Doc` events have (like MapEvent).

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlText" /> instance.
/// </summary>
public class XmlTextEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlTextEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlTextEvent(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the changes that triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventDelta" /> for more information.</para>
    /// </remarks>
    public EventDeltas Delta
    {
        get
        {
            var handle = XmlTextChannel.ObserveEventDelta(Handle, out var length);

            return new EventDeltas(handle, length);
        }
    }

    /// <summary>
    ///     Gets the attributes that changed and triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventKeys" /> for more information.</para>
    /// </remarks>
    public EventKeys Keys
    {
        get
        {
            var handle = XmlTextChannel.ObserveEventKeys(Handle, out var length);

            return new EventKeys(handle, length);
        }
    }

    /// <summary>
    ///     Gets the <see cref="XmlText" /> instance that is related to this event.
    /// </summary>
    public XmlText? Target => ReferenceAccessor.Access(new XmlText(XmlTextChannel.ObserveEventTarget(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
