using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Types.Texts;

namespace YDotNet.Document.Types.Texts.Events;

/// <summary>
///     Represents the event that's part of an operation within a <see cref="Text" /> instance.
/// </summary>
public class TextEvent : UnmanagedResource
{
    private readonly Lazy<EventDeltas> deltas;
    private readonly Lazy<EventPath> path;
    private readonly Lazy<Text> target;

    internal TextEvent(nint handle, Doc doc)
        : base(handle)
    {
        path = new Lazy<EventPath>(
            () =>
            {
                ThrowIfDisposed();

                var pathHandle = TextChannel.ObserveEventPath(handle, out var length).Checked();

                return new EventPath(pathHandle, length);
            });

        deltas = new Lazy<EventDeltas>(
            () =>
            {
                ThrowIfDisposed();

                var deltaHandle = TextChannel.ObserveEventDelta(handle, out var length).Checked();

                return new EventDeltas(deltaHandle, length, doc);
            });

        target = new Lazy<Text>(
            () =>
            {
                ThrowIfDisposed();

                var targetHandle = TextChannel.ObserveEventTarget(handle).Checked();

                return doc.GetText(targetHandle, isDeleted: false);
            });
    }

    /// <summary>
    ///     Gets the keys that changed within the <see cref="Text" /> instance and triggered this event.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventDeltas Delta => deltas.Value;

    /// <summary>
    ///     Gets the path from the observed instanced down to the current <see cref="Text" /> instance.
    /// </summary>
    /// <remarks>This property can only be accessed during the callback that exposes this instance.</remarks>
    public EventPath Path => path.Value;

    /// <summary>
    ///     Gets the <see cref="Text" /> instance that is related to this <see cref="TextEvent" /> instance.
    /// </summary>
    /// <returns>The target of the event.</returns>
    public Text Target => target.Value;

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        // The event has no explicit garbage collection, but is released automatically after the event has been completed.
    }
}
