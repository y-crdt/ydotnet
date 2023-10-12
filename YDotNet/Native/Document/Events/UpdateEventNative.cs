using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct UpdateEventNative
{
    public uint Length { get; init; }

    public nint Data { get; init; }

    public byte[] Bytes()
    {
        return MemoryReader.ReadBytes(Data, Length);
    }

    public static UpdateEventNative From(uint length, nint data)
    {
        return new UpdateEventNative { Length = length, Data = data };
    }
}
