using System.Collections.ObjectModel;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the path from the root type to the shared type that emitted the event related to this
///     <see cref="EventPath" /> instance.
/// </summary>
public class EventPath : ReadOnlyCollection<EventPathSegment>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventPath" /> class.
    /// </summary>
    /// <param name="handle">The handle to the beginning of the array of <see cref="EventPathSegment" /> instances.</param>
    /// <param name="length">The length of the array.</param>
    internal EventPath(nint handle, uint length)
        : base(ReadItems(handle, length))
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    private static IList<EventPathSegment> ReadItems(nint handle, uint length)
    {
        var result = MemoryReader.ReadIntPtrArray(handle, length, size: 16).Select(x => new EventPathSegment(x)).ToList();

        // We are done reading and can release the memory.
        PathChannel.Destroy(handle, length);
        return result;
    }
}
