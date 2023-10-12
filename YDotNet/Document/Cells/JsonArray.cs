using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

namespace YDotNet.Document.Cells;

/// <summary>
/// Represents a json array.
/// </summary>
public sealed class JsonArray : ReadOnlyCollection<Output>
{
    internal JsonArray(nint handle, uint length, Doc doc, IResourceOwner owner)
        : base(ReadItems(handle, length, doc, owner))
    {
    }

    private static List<Output> ReadItems(nint handle, uint length, Doc doc, IResourceOwner owner)
    {
        var collectionHandle = OutputChannel.Collection(handle);
        var collectionNatives = MemoryReader.ReadIntPtrArray(collectionHandle, length, Marshal.SizeOf<OutputNative>());

        var result = new List<Output>();

        foreach (var itemHandle in collectionNatives)
        {
            // The outputs are owned by this block of allocated memory.
            result.Add(new Output(itemHandle, doc, owner));
        }

        return result;
    }
}
