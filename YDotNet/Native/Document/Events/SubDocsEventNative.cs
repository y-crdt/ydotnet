using System.Runtime.InteropServices;
using YDotNet.Document;
using YDotNet.Document.Events;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal struct SubDocsEventNative
{
    public uint AddedLength { get; }

    public uint RemovedLength { get; }

    public uint LoadedLength { get; }

    public nint Added { get; }

    public nint Removed { get; }

    public nint Loaded { get; }

    public SubDocsEvent ToSubDocsEvent()
    {
        var added = new Doc[AddedLength];
        var removed = new Doc[RemovedLength];
        var loaded = new Doc[LoadedLength];

        for (var i = 0; i < AddedLength; i++)
        {
            added[i] = new Doc(Marshal.ReadIntPtr(Added, i * nint.Size));
        }

        for (var i = 0; i < RemovedLength; i++)
        {
            removed[i] = new Doc(Marshal.ReadIntPtr(Removed, i * nint.Size));
        }

        for (var i = 0; i < LoadedLength; i++)
        {
            loaded[i] = new Doc(Marshal.ReadIntPtr(Loaded, i * nint.Size));
        }

        return new SubDocsEvent(added, removed, loaded);
    }
}
