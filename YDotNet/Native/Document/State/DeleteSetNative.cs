using System.Runtime.InteropServices;
using YDotNet.Document.State;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DeleteSetNative
{
    public uint EntriesCount { get; }

    public nint ClientIds { get; }

    public nint Ranges { get; }

    public DeleteSet ToDeleteSet()
    {
        var nativeClients = MemoryReader.ReadStructArray<ulong>(ClientIds, EntriesCount);
        var nativeRanges = MemoryReader.ReadStructArray<IdRangeSequenceNative>(Ranges, EntriesCount);

        var entries = new Dictionary<ulong, IdRange[]>();

        for (var i = 0; i < EntriesCount; i++)
        {
            entries.Add(nativeClients[i], nativeRanges[i].ToIdRanges());
        }

        return new DeleteSet(entries);
    }
}
