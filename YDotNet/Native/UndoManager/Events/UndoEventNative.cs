using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.UndoManager.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct UndoEventNative
{
    public UndoEventKindNative KindNative { get; set; }

    public nint OriginHandle { get; set; }

    public uint OriginLength { get; set; }

    public byte[]? Origin()
    {
        if (OriginHandle == nint.Zero || OriginLength <= 0)
        {
            return null;
        }

        return MemoryReader.ReadBytes(OriginHandle, OriginLength);
    }
}
