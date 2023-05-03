using System.Runtime.InteropServices;
using YDotNet.Document.State;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
public class StateVectorNative
{
    public uint EntriesCount { get; init; }

    public ulong[] ClientIds { get; init; }

    public nint Clocks { get; init; }

    public StateVector ToStateVector()
    {
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
