using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Cells;

/// <summary>
/// Represents a json object.
/// </summary>
public sealed class JsonObject : ReadOnlyDictionary<string, Output>
{
    internal JsonObject(nint handle, uint length, Doc doc)
        : base(ReadItems(handle, length, doc))
    {
    }

    private static Dictionary<string, Output> ReadItems(nint handle, uint length, Doc doc)
    {
        var entriesHandle = OutputChannel.Object(handle).Checked();

        var result = new Dictionary<string, Output>();

        foreach (var (native, ptr) in MemoryReader.ReadIntPtrArray<MapEntryNative>(entriesHandle, length))
        {
            result[native.Key()] = new Output(native.ValueHandle(ptr), doc);
        }

        return result;
    }
}
