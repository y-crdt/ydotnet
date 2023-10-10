using System.Runtime.InteropServices;
using YDotNet.Document.Events;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct UpdateEventNative
{
    public uint Length { get; init; }

    public nint Data { get; init; }

    public static UpdateEventNative From(uint length, nint data)
    {
        return new UpdateEventNative
        {
            Length = length,
            Data = data
        };
    }

    public UpdateEvent ToUpdateEvent()
    {
        var result = new byte[Length];
        Marshal.Copy(Data, result, startIndex: 0, (int)Length);

        return new UpdateEvent(result);
    }
}
