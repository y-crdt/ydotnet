using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Cells;

public sealed class JsonObject : ReadOnlyDictionary<string, Output>
{
    public JsonObject(nint handle, uint length, IResourceOwner owner)
        : base(ReadItems(handle, length, owner))
    {
    }

    private static Dictionary<string, Output> ReadItems(nint handle, uint length, IResourceOwner owner)
    {
        var entriesHandle = OutputChannel.Object(handle).Checked();
        var entriesNatives = MemoryReader.ReadIntPtrArray(entriesHandle, length, Marshal.SizeOf<MapEntryNative>());

        var result = new Dictionary<string, Output>();

        foreach (var itemHandle in entriesNatives)
        {
            var (mapEntry, outputHandle) = MemoryReader.ReadMapEntryAndOutputHandle(itemHandle);
            var mapEntryKey = MemoryReader.ReadUtf8String(mapEntry.Field);

            result[mapEntryKey] = new Output(outputHandle, owner);
        }

        return result;
    }
}
