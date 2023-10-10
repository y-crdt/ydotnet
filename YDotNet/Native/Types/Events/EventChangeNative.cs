using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventChangeNative
{
    public EventChangeTagNative TagNative { get; }

    public uint Length { get; }

    public nint Values { get; }

    public EventChange ToEventChange()
    {
        var tag = TagNative switch
        {
            EventChangeTagNative.Add => EventChangeTag.Add,
            EventChangeTagNative.Remove => EventChangeTag.Remove,
            EventChangeTagNative.Retain => EventChangeTag.Retain,
            _ => throw new NotSupportedException($"The value \"{TagNative}\" for {nameof(EventChangeTagNative)} is not supported.")
        };

        var values =
            MemoryReader.TryReadIntPtrArray(Values, Length, Marshal.SizeOf<OutputNative>())?
                .Select(x => new Output(x, false)).ToList() ?? new List<Output>();

        return new EventChange(tag, Length, values);
    }
}
