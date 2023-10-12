using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

internal readonly struct StateVectorNative
{
    public uint EntriesCount { get; }

    public nint ClientIdsHandle { get; }

    public nint ClocksHandle { get; }

    public ulong[] ClientIds()
    {
        return MemoryReader.ReadStructArray<ulong>(ClientIdsHandle, EntriesCount);
    }

    public uint[] Clocks()
    {
        return MemoryReader.ReadStructArray<uint>(ClientIdsHandle, EntriesCount);
    }
}
