using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventDeltaNative
{
    public EventDeltaTagNative TagNative { get; }

    public uint Length { get; }

    public nint InsertHandle { get; }

    public uint AttributesLength { get; }

    public nint AttributesHandle { get; }

    public NativeWithHandle<MapEntryNative>[] Attributes
    {
        get
        {
            if (AttributesHandle == nint.Zero || AttributesLength == 0)
            {
                return Array.Empty<NativeWithHandle<MapEntryNative>>();
            }

            return MemoryReader.ReadIntPtrArray<MapEntryNative>(AttributesHandle, AttributesLength).ToArray();
        }
    }
}
