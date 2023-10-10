using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventDelta" /> instances.
/// </summary>
public class EventDeltas : ReadOnlyCollection<EventDelta>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventDeltas" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="length">The length of the array of <see cref="EventDelta" /> to read from <see cref="Handle" />.</param>
    internal EventDeltas(nint handle, uint length)
        : base(ReadItems(handle, length))
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    private static IList<EventDelta> ReadItems(nint handle, uint length)
    {
        var result = MemoryReader.ReadIntPtrArray(handle, length, Marshal.SizeOf<EventDeltaNative>())
            .Select(Marshal.PtrToStructure<EventDeltaNative>)
            .Select(x => x.ToEventDelta())
            .ToList();

        // We are done reading and can release the memory.
        EventChannel.DeltaDestroy(handle, length);

        return result;
    }
}
