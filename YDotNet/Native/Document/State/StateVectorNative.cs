using YDotNet.Document.State;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

internal readonly struct StateVectorNative
{
    public uint EntriesCount { get; }

    public nint ClientIds { get; }

    public nint Clocks { get; }

    public StateVector ToStateVector()
    {
        var nativeClients = MemoryReader.ReadStructArray<ulong>(ClientIds, EntriesCount);
        var nativeClocks = MemoryReader.ReadStructArray<uint>(Clocks, EntriesCount);

        var entries = new Dictionary<ulong, uint>();

        for (var i = 0; i < EntriesCount; i++)
        {
            entries.Add(nativeClients[i], nativeClocks[i]);
        }

        return new StateVector(entries);
    }
}
