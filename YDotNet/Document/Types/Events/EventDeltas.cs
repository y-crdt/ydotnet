using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventDelta" /> instances.
/// </summary>
public class EventDeltas : ReadOnlyCollection<EventDelta>
{
    internal EventDeltas(nint handle, uint length, Doc doc)
        : base(ReadItems(handle, length, doc))
    {
    }

    private static IList<EventDelta> ReadItems(nint handle, uint length, Doc doc)
    {
        var result = new List<EventDelta>((int) length);

        foreach (var native in MemoryReader.ReadStructs<EventDeltaNative>(handle, length))
        {
            result.Add(new EventDelta(native, doc));
        }

        // We are done reading and can destroy the resource.
        EventChannel.DeltaDestroy(handle, length);

        return result;
    }
}
