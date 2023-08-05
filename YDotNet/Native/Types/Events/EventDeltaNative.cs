using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct EventDeltaNative
{
    public EventDeltaTagNative TagNative { get; }

    public uint Length { get; }

    public nint InsertHandle { get; }

    public uint AttributesLength { get; }

    public nint Attributes { get; }

    public EventDelta ToEventDelta()
    {
        var tag = TagNative switch
        {
            EventDeltaTagNative.Add => EventDeltaTag.Add,
            EventDeltaTagNative.Remove => EventDeltaTag.Remove,
            EventDeltaTagNative.Retain => EventDeltaTag.Retain,
            _ => throw new NotSupportedException(
                $"The value \"{TagNative}\" for {nameof(EventDeltaTagNative)} is not supported.")
        };

        var attributes = MemoryReader.TryReadIntPtrArray(Attributes, AttributesLength, size: 16)
                             ?.Select(x => new EventDeltaAttribute(x))
                             .ToArray() ??
                         Enumerable.Empty<EventDeltaAttribute>();

        return new EventDelta(
            tag,
            Length,
            InsertHandle == nint.Zero ? null : new Output(InsertHandle),
            attributes
        );
    }
}
