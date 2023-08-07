using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Texts;

namespace YDotNet.Document.Types.Texts.Events;

// TODO [LSViana] Check if this event should have a `*Native` counterpart like `Doc` events have (like MapEvent).

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Text" /> instance.
/// </summary>
public class TextEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TextEvent" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal TextEvent(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the keys that changed within the <see cref="Text" /> instance and triggered this event.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventDelta" /> for more information.</para>
    /// </remarks>
    public EventDeltas Delta
    {
        get
        {
            var handle = TextChannel.ObserveEventDelta(Handle, out var length);

            return new EventDeltas(handle, length);
        }
    }

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="Text" /> instance.
    /// </summary>
    /// <remarks>
    ///     <para>This property can only be accessed during the callback that exposes this instance.</para>
    ///     <para>Check the documentation of <see cref="EventPath" /> for more information.</para>
    /// </remarks>
    public EventPath Path
    {
        get
        {
            var handle = TextChannel.ObserveEventPath(Handle, out var length);

            return new EventPath(handle, length);
        }
    }

    /// <summary>
    ///     Gets the <see cref="Text" /> instance that is related to this <see cref="TextEvent" /> instance.
    /// </summary>
    public Text? Target => ReferenceAccessor.Access(new Text(TextChannel.ObserveEventTarget(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
