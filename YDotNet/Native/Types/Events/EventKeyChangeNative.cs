using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Types.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct EventKeyChangeNative
{
    public nint KeyHandle { get; }

    public EventKeyChangeTagNative TagNative { get; }

    public nint OldValue { get; }

    public nint NewValue { get; }

    public string Key()
    {
        return MemoryReader.ReadUtf8String(KeyHandle);
    }
}
