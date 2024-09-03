using YDotNet.Native.UndoManager.Events;

namespace YDotNet.Document.UndoManagers.Events;

/// <summary>
///     Represents a redo/undo event from an <see cref="UndoManager" />.
/// </summary>
public class UndoEvent
{
    internal UndoEvent(UndoEventNative native)
    {
        Origin = native.Origin();

        Kind = native.KindNative switch
        {
            UndoEventKindNative.Undo => UndoEventKind.Undo,
            UndoEventKindNative.Redo => UndoEventKind.Redo,
            _ => throw new NotSupportedException($"The value \"{native.KindNative}\" for {nameof(UndoEventKindNative)} is not supported.")
        };
    }

    /// <summary>
    ///     Gets the kind of the event.
    /// </summary>
    public UndoEventKind Kind { get; }

    /// <summary>
    ///     Gets the origin of the event.
    /// </summary>
    /// <remarks>
    ///     The <see cref="Origin" /> is a binary marker.
    /// </remarks>
    public byte[]? Origin { get; }
}
