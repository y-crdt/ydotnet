using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventChange" /> instances.
/// </summary>
public class EventChanges : ReadOnlyCollection<EventChange>
{
    internal EventChanges(nint handle, uint length, Doc doc)
        : base(ReadItems(handle, length, doc))
    {
    }

    private static IList<EventChange> ReadItems(nint handle, uint length, Doc doc)
    {
        var result = new List<EventChange>();

        foreach (var native in MemoryReader.ReadStructs<EventChangeNative>(handle, length))
        {
            result.Add(new EventChange(native, doc));
        }

        // We are done reading and can release the resource.
        EventChannel.DeltaDestroy(handle, length);

        return result;
    }
}
