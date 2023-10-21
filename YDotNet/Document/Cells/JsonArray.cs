using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a JSON array.
/// </summary>
public sealed class JsonArray : ReadOnlyCollection<Output>
{
    internal JsonArray(nint handle, uint length, Doc doc, bool isDeleted)
        : base(ReadItems(handle, length, doc, isDeleted))
    {
    }

    private static List<Output> ReadItems(nint handle, uint length, Doc doc, bool isDeleted)
    {
        var collectionHandle = OutputChannel.Collection(handle);
        var collectionNatives = MemoryReader.ReadPointers<OutputNative>(collectionHandle, length);

        var result = new List<Output>();

        foreach (var itemHandle in collectionNatives)
        {
            result.Add(new Output(itemHandle, doc, isDeleted));
        }

        return result;
    }
}
