using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the keys that changed the shared type that emitted the event related to this <see cref="EventKeys" /> instance.
/// </summary>
public class EventKeys : ReadOnlyCollection<EventKeyChange>
{
    internal EventKeys(nint handle, uint length, Doc doc)
        : base(ReadItems(handle, length, doc))
    {
    }

    private static IList<EventKeyChange> ReadItems(nint handle, uint length, Doc doc)
    {
        var result = new List<EventKeyChange>();

        foreach (var native in MemoryReader.ReadStructsWithHandles<EventKeyChangeNative>(handle, length))
        {
            result.Add(new EventKeyChange(native.Value, doc));
        }

        // We are done reading and can destroy the resource.
        EventChannel.KeysDestroy(handle, length);

        return result;
    }
}
