using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the path from the root type to the shared type that emitted the event related to this
///     <see cref="EventPath" /> instance.
/// </summary>
public sealed class EventPath : ReadOnlyCollection<EventPathSegment>
{
    internal EventPath(nint handle, uint length)
        : base(ReadItems(handle, length))
    {
    }

    private static IList<EventPathSegment> ReadItems(nint handle, uint length)
    {
        var result = new List<EventPathSegment>();

        foreach (var native in MemoryReader.ReadStructs<EventPathSegmentNative>(handle, length))
        {
            result.Add(new EventPathSegment(native));
        }

        // We have read everything, so we can release the memory immediately.
        PathChannel.Destroy(handle, length);

        return result;
    }
}
