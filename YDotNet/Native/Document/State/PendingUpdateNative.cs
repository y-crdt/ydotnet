using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct PendingUpdateNative
{
    public StateVectorNative Missing { get; }

    public nint UpdateHandle { get; }

    public uint UpdateLength { get; }

    public byte[] Update()
    {
        return MemoryReader.ReadBytes(UpdateHandle, UpdateLength);
    }
}
