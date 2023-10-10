using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements.Events;

/// <summary>
///     Represents the event that's part of an operation within an <see cref="XmlElement" /> instance.
/// </summary>
public class XmlElementEvent
{
    private readonly Lazy<EventPath> path;
    private readonly Lazy<EventChanges> delta;
    private readonly Lazy<EventKeys> keys;
    private readonly Lazy<XmlElement> target;

    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlElementEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlElementEvent(nint handle)
    {
        Handle = handle;

        path = new Lazy<EventPath>(() =>
        {
            var pathHandle = XmlElementChannel.ObserveEventPath(handle, out var length).Checked();
            return new EventPath(pathHandle, length);
        });

        delta = new Lazy<EventChanges>(() =>
        {
            var deltaHandle = XmlElementChannel.ObserveEventDelta(handle, out var length).Checked();
            return new EventChanges(deltaHandle, length);
        });

        keys = new Lazy<EventKeys>(() =>
        {
            var keysHandle = XmlElementChannel.ObserveEventKeys(handle, out var length).Checked();
            return new EventKeys(keysHandle, length);
        });

        target = new Lazy<XmlElement>(() =>
        {
            var targetHandle = XmlElementChannel.ObserveEventTarget(handle).Checked();
            return new XmlElement(targetHandle);
        });
    }

    /// <summary>
    ///     Gets the changes within the <see cref="XmlElement" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventChanges Delta => delta.Value;

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventPath Path => path.Value;

    /// <summary>
    ///     Gets the attributes that changed within the <see cref="XmlElement" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventKeys Keys => keys.Value;

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> instance that is related to this <see cref="XmlElementEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    /// <remarks>You are responsible to dispose the text, if you use this property.</remarks>
    public XmlElement ResolveTarget() => target.Value;
}
