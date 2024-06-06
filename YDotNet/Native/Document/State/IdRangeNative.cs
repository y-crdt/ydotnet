using System.Runtime.InteropServices;

namespace YDotNet.Native.Document.State;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct IdRangeNative
{
    public uint Start { get; }

    public uint End { get; }
}
