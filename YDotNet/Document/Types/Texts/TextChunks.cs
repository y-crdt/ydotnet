using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     Represents a collection of <see cref="TextChunk" /> instances.
/// </summary>
public class TextChunks : ReadOnlyCollection<TextChunk>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TextChunks" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="length">The length of <see cref="TextChunk" /> instances to be read from <see cref="Handle" />.</param>
    internal TextChunks(nint handle, uint length)
        : base(ReadItems(handle, length))
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    private static IList<TextChunk> ReadItems(nint handle, uint length)
    {
        var result = MemoryReader.ReadIntPtrArray(handle, length, size: 32).Select(x => new TextChunk(x)).ToList();

        // We are done reading and can release the memory.
        // ChunksChannel.Destroy(handle, length);

        return result;
    }
}
