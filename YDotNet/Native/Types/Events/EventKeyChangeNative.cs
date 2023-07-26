using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct EventKeyChangeNative
{
    public string Key { get; }

    public EventKeyChangeTagNative TagNative { get; }

    public nint OldValue { get; }

    public nint NewValue { get; }

    public EventKeyChange ToMapEventKeyChange()
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
            Key,
            tag,
            OldValue == nint.Zero ? null : new Output(OldValue),
            NewValue == nint.Zero ? null : new Output(NewValue)
        );

        return result;
    }
}
