using System.Runtime.InteropServices;
using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
public class DeleteSetNative
{
    public uint EntriesCount { get; init; }

    public nint ClientIds { get; init; }

    public nint Ranges { get; init; }

    public DeleteSet ToDeleteSet()
    {
        var entries = new Dictionary<ulong, IdRange[]>();

        for (var i = 0; i < EntriesCount; i++)
        {
            var clientId = (ulong) Marshal.ReadInt64(ClientIds, i * sizeof(ulong));
            var rangeNative =
                Marshal.PtrToStructure<IdRangeSequenceNative>(Ranges + i * Marshal.SizeOf<IdRangeSequenceNative>());

            var range = rangeNative.ToIdRanges();

            entries.Add(clientId, range);
        }

        return new DeleteSet(entries);
    }
}
