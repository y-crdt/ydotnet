using System.Runtime.InteropServices;
using YDotNet.Document.Options;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DocOptionsNative
{
    public ulong Id { get; init; }

    public nint Guid { get; init; }

    public nint CollectionId { get; init; }

    public byte Encoding { get; init; }

    public byte SkipGc { get; init; }

    public byte AutoLoad { get; init; }

    public byte ShouldLoad { get; init; }

    public static DocOptionsNative From(DocOptions options)
    {
        // We can never release the memory because y-crdt just receives a pointer to that.
        var unsafeGuid = MemoryWriter.WriteUtf8String(options.Guid);
        var unsafeCollection = MemoryWriter.WriteUtf8String(options.CollectionId);

        return new DocOptionsNative
        {
            Id = options.Id ?? 0,
            Guid = unsafeGuid.Handle,
            CollectionId = unsafeCollection.Handle,
            Encoding = (byte)options.Encoding,
            SkipGc = (byte)(options.SkipGarbageCollection ?? false ? 1 : 0),
            AutoLoad = (byte)(options.AutoLoad ?? false ? 1 : 0),
            ShouldLoad = (byte)(options.ShouldLoad ?? false ? 1 : 0)
        };
    }
}
