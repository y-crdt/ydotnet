using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct EventKeyChangeNative
{
    public nint Key { get; }

    public EventKeyChangeTagNative TagNative { get; }

    public nint OldValue { get; }

    public nint NewValue { get; }

    public EventKeyChange ToEventKeyChange()
    {
        var tag = TagNative switch
        {
            EventKeyChangeTagNative.Add => EventKeyChangeTag.Add,
            EventKeyChangeTagNative.Remove => EventKeyChangeTag.Remove,
            EventKeyChangeTagNative.Update => EventKeyChangeTag.Update,
            _ => throw new NotSupportedException(
                $"The value \"{TagNative}\" for {nameof(EventKeyChangeTagNative)} is not supported.")
        };

        var result = new EventKeyChange(
            MemoryReader.ReadUtf8String(Key),
            tag,
            ReferenceAccessor.Access(new Output(OldValue)),
            ReferenceAccessor.Access(new Output(NewValue))
        );

        return result;
    }
}
