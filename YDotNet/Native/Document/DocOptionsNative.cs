using System.Runtime.InteropServices;
using YDotNet.Document.Options;

namespace YDotNet.Native.Document;

[StructLayout(LayoutKind.Sequential)]
internal struct DocOptionsNative
{
    public ulong Id { get; init; }

    public string? Guid { get; init; }

    public string? CollectionId { get; init; }

    public byte Encoding { get; init; }

    public byte SkipGc { get; init; }

    public byte AutoLoad { get; init; }

    public byte ShouldLoad { get; init; }

    public static DocOptionsNative From(DocOptions options)
    {
        return new DocOptionsNative
        {
            Id = options.Id ?? 0,
            Guid = options.Guid,
            CollectionId = options.CollectionId,
            Encoding = (byte) (options.Encoding ?? DocEncoding.Utf8),
            SkipGc = (byte) (options.SkipGarbageCollection ?? false ? 1 : 0),
            AutoLoad = (byte) (options.AutoLoad ?? false ? 1 : 0),
            ShouldLoad = (byte) (options.ShouldLoad ?? false ? 1 : 0)
        };
    }
}
