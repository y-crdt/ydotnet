using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     Represents a collection of <see cref="TextChunk" /> instances.
/// </summary>
public class TextChunks : UnmanagedCollectionResource<TextChunk>
{
    private readonly uint length;

    internal TextChunks(nint handle, uint length)
        : base(handle, null)
    {
        foreach (var chunkHandle in MemoryReader.ReadIntPtrArray(handle, length, size: 32))
        {
            // The cunks create output that are owned by this block of allocated memory.
            AddItem(new TextChunk(chunkHandle, this));
        }

        GC.Collect();

        this.length = length;
    }

    ~TextChunks()
    {
        Dispose(true);
    }

    protected override void DisposeCore(bool disposing)
    {

        Console.WriteLine("---DISPOSE {0} - {1}", Handle, length);
        Console.Out.Flush();
        Thread.Sleep(100);
        ChunksChannel.Destroy(Handle, length);
    }
}
