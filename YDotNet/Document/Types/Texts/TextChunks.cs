using System.Collections;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     Represents a collection of <see cref="TextChunk" /> instances.
/// </summary>
public class TextChunks : IEnumerable<TextChunk>, IDisposable
{
    private readonly IEnumerable<TextChunk> collection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TextChunks" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="length">The length of <see cref="TextChunk" /> instances to be read from <see cref="Handle" />.</param>
    internal TextChunks(nint handle, uint length)
    {
        Handle = handle;
        Length = length;

        collection = MemoryReader.TryReadIntPtrArray(Handle, Length, size: 32)
            .Select(x => new TextChunk(x))
            .ToArray();
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the length of the native resource.
    /// </summary>
    internal uint Length { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        ChunksChannel.Destroy(Handle, Length);
    }

    /// <inheritdoc />
    public IEnumerator<TextChunk> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
