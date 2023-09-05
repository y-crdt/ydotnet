using YDotNet.Document.UndoManagers.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Document.State;

namespace YDotNet.Native.UndoManager.Events;

internal struct UndoEventNative
{
    public UndoEventKindNative KindNative { get; set; }

    public nint Origin { get; set; }

    public uint OriginLength { get; set; }

    public DeleteSetNative Insertions { get; set; }

    public DeleteSetNative Deletions { get; set; }

    public UndoEvent ToUndoEvent()
    {
        var kind = KindNative switch
        {
            UndoEventKindNative.Undo => UndoEventKind.Undo,
            UndoEventKindNative.Redo => UndoEventKind.Redo,
            _ => throw new NotSupportedException(
                $"The value \"{KindNative}\" for {nameof(UndoEventKindNative)} is not supported.")
        };

        var origin = MemoryReader.TryReadBytes(Origin, OriginLength);

        return new UndoEvent(kind, origin, Insertions.ToDeleteSet(), Deletions.ToDeleteSet());
    }
}
