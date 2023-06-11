using System.Runtime.InteropServices;

namespace YDotNet.Native.Cells;

[StructLayout(LayoutKind.Sequential)]
internal struct Input
{
    public sbyte Tag { get; }

    public uint Length { get; }

    public InputContent Content { get; }
}
