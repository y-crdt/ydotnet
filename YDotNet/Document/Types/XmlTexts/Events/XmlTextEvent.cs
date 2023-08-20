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
    ///     Gets the <see cref="XmlText" /> instance that is related to this event.
    /// </summary>
    public XmlText? Target => ReferenceAccessor.Access(new XmlText(XmlTextChannel.ObserveEventTarget(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
