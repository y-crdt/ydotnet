using System.Runtime.InteropServices;
using YDotNet.Document;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventKeyChangeNative
{
    public nint Key { get; }

    public EventKeyChangeTagNative TagNative { get; }

    public nint OldValue { get; }

    public nint NewValue { get; }

    public EventKeyChange ToEventKeyChange(Doc doc, IResourceOwner owner)
    {
        var tag = TagNative switch
        {
            EventKeyChangeTagNative.Add => EventKeyChangeTag.Add,
            EventKeyChangeTagNative.Remove => EventKeyChangeTag.Remove,
            EventKeyChangeTagNative.Update => EventKeyChangeTag.Update,
            _ => throw new NotSupportedException($"The value \"{TagNative}\" for {nameof(EventKeyChangeTagNative)} is not supported."),
        };

        var key = MemoryReader.ReadUtf8String(Key);

        var oldOutput =
            OldValue != nint.Zero ?
            new Output(OldValue, doc, owner) :
            null;

        var newOutput =
            NewValue != nint.Zero ?
            new Output(NewValue, doc, owner) :
            null;

        return new EventKeyChange(key, tag, oldOutput, newOutput);
    }
}
