using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventChangeNative
{
    public EventChangeTagNative TagNative { get; }

    public uint Length { get; }

    public nint Values { get; }

    public nint[] ValuesHandles
    {
        get
        {
            if (Values == nint.Zero)
            {
                return Array.Empty<nint>();
            }

            return MemoryReader.ReadPointers<OutputNative>(Values, Length).ToArray();
        }
    }
}
