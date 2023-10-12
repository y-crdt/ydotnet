using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Texts;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     Represents a collection of <see cref="TextChunk" /> instances.
/// </summary>
public class TextChunks : ReadOnlyCollection<TextChunk>
{

    internal TextChunks(nint handle, uint length, Doc doc)
        : base(ReadItems(handle, length, doc))
    {
    }

    private static IList<TextChunk> ReadItems(nint handle, uint length, Doc doc)
    {
        var result = new List<TextChunk>((int)length);

        foreach (var native in MemoryReader.ReadIntPtrArray<TextChunkNative>(handle, length))
        {
            result.Add(new TextChunk(native, doc));
        }

        // We are done reading and can release the memory.
        ChunksChannel.Destroy(handle, length);

        return result;
    }
}
