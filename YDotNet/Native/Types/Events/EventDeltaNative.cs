using System.Runtime.InteropServices;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventDeltaNative
{
    public EventDeltaTagNative TagNative { get; }

    public uint Length { get; }

    public nint InsertHandle { get; }

    public uint AttributesLength { get; }

    public nint Attributes { get; }

    public EventDelta ToEventDelta(Doc doc, IResourceOwner owner)
    {
        var tag = TagNative switch
        {
            EventDeltaTagNative.Add => EventDeltaTag.Add,
            EventDeltaTagNative.Remove => EventDeltaTag.Remove,
            EventDeltaTagNative.Retain => EventDeltaTag.Retain,
            _ => throw new NotSupportedException($"The value \"{TagNative}\" for {nameof(EventDeltaTagNative)} is not supported."),
        };

        var attributes =
            Attributes != nint.Zero ?
            MemoryReader.ReadIntPtrArray(Attributes, AttributesLength, size: 16).Select(x => new EventDeltaAttribute(x, doc, owner)).ToList() :
            new List<EventDeltaAttribute>();

        var insert =
            InsertHandle != nint.Zero ?
            new Output(InsertHandle, doc, owner) :
            null;

        return new EventDelta(tag, Length, insert, attributes);
    }
}
