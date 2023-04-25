using System.Runtime.InteropServices;
using YDotNet.Document.Options;
using YDotNet.Native.Convert;

namespace YDotNet.Native.Doc;

[StructLayout(LayoutKind.Sequential)]
public class DocOptionsNative
{
    public ulong Id { get; init; }

    public byte[]? Guid { get; init; }

    public byte[]? CollectionId { get; init; }

    public byte Encoding { get; init; }

    public byte SkipGc { get; init; }

    public byte AutoLoad { get; init; }

    public byte ShouldLoad { get; init; }

    public static DocOptionsNative From(DocOptions options)
    {
        return new DocOptionsNative
        {
            Id = options.Id ?? 0,
            Guid = options.Guid?.ToString().ToUtf8Bytes(),
            CollectionId = options.CollectionId?.ToUtf8Bytes(),
            Encoding = (byte) (options.Encoding ?? DocEncoding.Utf8),
            SkipGc = (byte) (options.SkipGarbageCollection ?? false ? 1 : 0),
            AutoLoad = (byte) (options.AutoLoad ?? false ? 1 : 0),
            ShouldLoad = (byte) (options.ShouldLoad ?? false ? 1 : 0)
        };
    }
}
