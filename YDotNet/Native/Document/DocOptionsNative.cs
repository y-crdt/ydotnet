using System.Runtime.InteropServices;
using YDotNet.Document.Options;
using YDotNet.Infrastructure;

namespace YDotNet.Native.Document;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DocOptionsNative : IDisposable
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
        MemoryWriter.TryWriteUtf8String(options.Guid, out var guidHandle);
        MemoryWriter.TryWriteUtf8String(options.CollectionId, out var collectionIdHandle);

        return new DocOptionsNative
        {
            Id = options.Id ?? 0,
            Guid = guidHandle,
            CollectionId = collectionIdHandle,
            Encoding = (byte)(options.Encoding ?? DocEncoding.Utf8),
            SkipGc = (byte)(options.SkipGarbageCollection ?? false ? 1 : 0),
            AutoLoad = (byte)(options.AutoLoad ?? false ? 1 : 0),
            ShouldLoad = (byte)(options.ShouldLoad ?? false ? 1 : 0)
        };
    }

    /// <inheritdoc />
    public void Dispose()
    {
        MemoryWriter.TryRelease(Guid);
        MemoryWriter.TryRelease(CollectionId);
    }
}
