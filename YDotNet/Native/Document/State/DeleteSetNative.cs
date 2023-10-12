using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DeleteSetNative
{
    public uint EntriesCount { get; }

    public nint ClientIdsHandle { get; }

    public nint RangesHandle { get; }

    public ulong[] Clients()
    {
        return MemoryReader.ReadStructArray<ulong>(ClientIdsHandle, EntriesCount);
    }

    public IdRangeSequenceNative[] Ranges()
    {
        return MemoryReader.ReadStructArray<IdRangeSequenceNative>(RangesHandle, EntriesCount);
    }
}
