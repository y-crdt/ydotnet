using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventDeltaNative
{
    public EventDeltaTagNative TagNative { get; }

    public uint Length { get; }

    public uint AttributesLength { get; }

    public nint AttributesHandle { get; }

    public nint InsertHandle { get; }

    public NativeWithHandle<DeltaAttributeNative>[] Attributes
    {
        get
        {
            if (AttributesHandle == nint.Zero || AttributesLength == 0)
            {
                return Array.Empty<NativeWithHandle<DeltaAttributeNative>>();
            }

            return MemoryReader.ReadStructsWithHandles<DeltaAttributeNative>(AttributesHandle, AttributesLength).ToArray();
        }
    }
}
