using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct SubDocsEventNative
{
    public uint AddedLength { get; }

    public uint RemovedLength { get; }

    public uint LoadedLength { get; }

    public nint AddedHandle { get; }

    public nint RemovedHandle { get; }

    public nint LoadedHandle { get; }

    public nint[] Added()
    {
        if (AddedHandle == nint.Zero || AddedLength == 0)
        {
            return Array.Empty<nint>();
        }

        return MemoryReader.ReadStructs<nint>(AddedHandle, AddedLength);
    }

    public nint[] Removed()
    {
        if (RemovedHandle == nint.Zero || RemovedLength == 0)
        {
            return Array.Empty<nint>();
        }

        return MemoryReader.ReadStructs<nint>(RemovedHandle, RemovedLength);
    }

    public nint[] Loaded()
    {
        if (LoadedHandle == nint.Zero || LoadedLength == 0)
        {
            return Array.Empty<nint>();
        }

        return MemoryReader.ReadStructs<nint>(LoadedHandle, LoadedLength);
    }
}
