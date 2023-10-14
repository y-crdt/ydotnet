using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Document;

namespace YDotNet.Document.Options;

/// <summary>
///     Configuration object that, optionally, is used to create <see cref="Doc" /> instances.
/// </summary>
public class DocOptions
{
    /// <summary>
    ///     Gets the default options value used to initialize <see cref="Doc" /> instances.
    /// </summary>
    public static DocOptions Default => new()
    {
        Id = (ulong)Random.Shared.Next()
    };

    /// <summary>
    ///     Gets the globally unique 53-bit integer assigned to the <see cref="Doc" /> replica as its identifier.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If two clients share the same <see cref="Id" /> and will perform any updates, it will result in
    ///         unrecoverable <see cref="Doc" /> state corruption.
    ///     </para>
    ///     <para>
    ///         The same thing may happen if the client restored <see cref="Doc" /> state from snapshot, that didn't
    ///         contain all of that clients updates that were sent to other peers.
    ///     </para>
    /// </remarks>
    public ulong Id { get; init; }

    /// <summary>
    ///     Gets the globally unique UUID v4 compatible string identifier of this <see cref="Doc" />.
    /// </summary>
    /// <remarks>
    ///     If passed as <c>null</c>, a random UUID will be generated instead.
    /// </remarks>
    public string? Guid { get; init; }

    /// <summary>
    ///     Gets the UTF-8 encoded, string of a collection that this <see cref="Doc" /> belongs to.
    /// </summary>
    /// <remarks>
    ///     It's used only by providers.
    /// </remarks>
    public string? CollectionId { get; init; }

    /// <summary>
    ///     Gets the encoding used by text editing operations on this <see cref="Doc" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         It's used to compute <see cref="Text" /> and <see cref="XmlText" /> insertion offsets and string lengths.
    ///     </para>
    ///     <para>
    ///         Read more about the possible values in <see cref="DocEncoding" />.
    ///     </para>
    /// </remarks>
    public DocEncoding Encoding { get; init; } = DocEncoding.Utf16;

    /// <summary>
    ///     Gets a value indicating whether deleted blocks should be garbage collected during transaction commits.
    /// </summary>
    /// <remarks>
    ///     Setting this value to <c>false</c> means garbage collection will be performed.
    /// </remarks>
    public bool SkipGarbageCollection { get; init; }

    /// <summary>
    ///     Gets a value indicating whether sub-documents should be loaded automatically.
    /// </summary>
    /// <remarks>
    ///     If this is a sub-document, remote peers will automatically load the <see cref="Doc" /> as well.
    /// </remarks>
    public bool AutoLoad { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the <see cref="Doc" /> should be synchronized by the provider now.
    /// </summary>
    public bool ShouldLoad { get; init; } = true;

    internal DocOptionsNative ToNative()
    {
        // We can never release the memory because y-crdt just receives a pointer to that.
        var unsafeGuid = MemoryWriter.WriteUtf8String(Guid);
        var unsafeCollection = MemoryWriter.WriteUtf8String(CollectionId);

        return new DocOptionsNative
        {
            Id = Id,
            Guid = unsafeGuid.Handle,
            CollectionId = unsafeCollection.Handle,
            Encoding = (byte)Encoding,
            SkipGc = (byte)(SkipGarbageCollection ? 1 : 0),
            AutoLoad = (byte)(AutoLoad ? 1 : 0),
            ShouldLoad = (byte)(ShouldLoad ? 1 : 0)
        };
    }
}
