using System.Runtime.InteropServices;
using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DeleteSetNative
{
    public uint EntriesCount { get; }

    public nint ClientIds { get; }

    public nint Ranges { get; }

    public DeleteSet ToDeleteSet()
    {
        var longSize = sizeof(ulong);
        var idRangeSequenceSize = Marshal.SizeOf<IdRangeSequenceNative>();
        var entries = new Dictionary<ulong, IdRange[]>();

        for (var i = 0; i < EntriesCount; i++)
        {
            var clientId = (ulong)Marshal.ReadInt64(ClientIds, i * longSize);
            var rangeNative = Marshal.PtrToStructure<IdRangeSequenceNative>(Ranges + i * idRangeSequenceSize);

            var range = rangeNative.ToIdRanges();

            entries.Add(clientId, range);
        }

        return new DeleteSet(entries);
    }
}
