using System.Runtime.InteropServices;
using YDotNet.Document;
using YDotNet.Document.Events;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct SubDocsEventNative
{
    public uint AddedLength { get; }

    public uint RemovedLength { get; }

    public uint LoadedLength { get; }

    public nint Added { get; }

    public nint Removed { get; }

    public nint Loaded { get; }

    public SubDocsEvent ToSubDocsEvent(Doc doc)
    {
        var nativeAdded = MemoryReader.ReadStructArray<nint>(Added, AddedLength);
        var nativeRemoved = MemoryReader.ReadStructArray<nint>(Removed, RemovedLength);
        var nativeLoaded = MemoryReader.ReadStructArray<nint>(Loaded, LoadedLength);

        var docsAdded = nativeAdded.Select(doc.GetDoc).ToArray();
        var docsRemoved = nativeRemoved.Select(doc.GetDoc).ToArray();
        var docsLoaded = nativeLoaded.Select(doc.GetDoc).ToArray();

        return new SubDocsEvent(docsAdded, docsRemoved, docsLoaded);
    }
}
