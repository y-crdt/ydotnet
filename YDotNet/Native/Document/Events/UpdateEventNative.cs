using System.Runtime.InteropServices;
using YDotNet.Document.Events;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct UpdateEventNative
{
    public uint Length { get; init; }

    public nint Data { get; init; }

    public static UpdateEventNative From(uint length, nint data)
    {
        return new UpdateEventNative { Length = length, Data = data };
    }

    public UpdateEvent ToUpdateEvent()
    {
        var result = MemoryReader.ReadBytes(Data, Length);

        return new UpdateEvent(result);
    }
}
