using System.Runtime.InteropServices;
using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Explicit, Size = 24)]
internal struct StateVectorNative
{
    [field: FieldOffset(0)]
    public uint EntriesCount { get; }

    [field: FieldOffset(8)]
    public nint ClientIds { get; }

    [field: FieldOffset(16)]
    public nint Clocks { get; }

    public StateVector ToStateVector()
    {
        Console.WriteLine("Entries {0}", EntriesCount);

        var entries = new Dictionary<ulong, uint>();

        for (var i = 0; i < EntriesCount; i++)
        {
            var clientId = (ulong) Marshal.ReadInt64(ClientIds, i * sizeof(ulong));
            var clock = (uint) Marshal.ReadInt32(Clocks, i * sizeof(uint));

            entries.Add(clientId, clock);
        }

        return new StateVector(entries);
    }
}
