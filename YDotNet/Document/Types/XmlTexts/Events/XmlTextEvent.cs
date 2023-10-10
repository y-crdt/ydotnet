using System.IO;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlTexts.Events;

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlText" /> instance.
/// </summary>
public class XmlTextEvent
{
    private readonly Lazy<EventDeltas> delta;
    private readonly Lazy<EventKeys> keys;
    private readonly Lazy<XmlText> target;

    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlTextEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlTextEvent(nint handle)
    {
        Handle = handle;

        delta = new Lazy<EventDeltas>(() =>
        {
            var deltaHandle = XmlTextChannel.ObserveEventDelta(handle, out var length).Checked();
            return new EventDeltas(deltaHandle, length);
        });

        keys = new Lazy<EventKeys>(() =>
        {
            var keysHandle = XmlTextChannel.ObserveEventKeys(handle, out var length).Checked();
            return new EventKeys(keysHandle, length);
        });

        target = new Lazy<XmlText>(() =>
        {
            var targetHandle = XmlTextChannel.ObserveEventTarget(handle).Checked();
            return new XmlText(targetHandle);
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
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the <see cref="XmlText" /> instance that is related to this <see cref="XmlTextEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    /// <remarks>You are responsible to dispose the text, if you use this property.</remarks>
    public XmlText ResolveTarget() => target.Value;
}
