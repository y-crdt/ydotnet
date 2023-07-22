using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Maps.Events;

namespace YDotNet.Native.Types.Maps.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct MapEventKeyChangeNative
{
    public string Key { get; }

    public MapEventKeyChangeTagNative TagNative { get; }

    public nint OldValue { get; }

    public nint NewValue { get; }

    public MapEventKeyChange ToMapEventKeyChange()
    {
        var tag = TagNative switch
        {
            MapEventKeyChangeTagNative.Add => MapEventKeyChangeTag.Add,
            MapEventKeyChangeTagNative.Remove => MapEventKeyChangeTag.Remove,
            MapEventKeyChangeTagNative.Update => MapEventKeyChangeTag.Update,
            _ => throw new NotSupportedException(
                $"The value \"{TagNative}\" for {nameof(MapEventKeyChangeTagNative)} is not supported.")
        };

        var result = new MapEventKeyChange(
            Key,
            tag,
            OldValue == nint.Zero ? null : new Output(OldValue),
            NewValue == nint.Zero ? null : new Output(NewValue)
        );

        return result;
    }
}
