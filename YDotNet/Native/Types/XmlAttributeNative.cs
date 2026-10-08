using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Native.Types;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct XmlAttributeNative
{
    public nint KeyHandle { get; }

    public nint ValueHandle { get; }

    public string Key()
    {
        return MemoryReader.ReadUtf8String(KeyHandle);
    }

    public string Value()
    {
        // The value is stored as an output cell, but attributes are always strings.
        return MemoryReader.ReadUtf8String(OutputChannel.String(ValueHandle).Checked());
    }
}
