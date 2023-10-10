using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventChange" /> instances.
/// </summary>
public class EventChanges : ReadOnlyCollection<EventChange>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventChanges" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="length">The length of the array of <see cref="EventChange" /> to read from <see cref="Handle" />.</param>
    public EventChanges(nint handle, uint length)
        : base(ReadItems(handle, length))
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    private static IList<EventChange> ReadItems(nint handle, uint length)
    {
        var result = MemoryReader.TryReadIntPtrArray(handle, length, Marshal.SizeOf<EventChangeNative>())!
            .Select(Marshal.PtrToStructure<EventChangeNative>)
            .Select(x => x.ToEventChange())
            .ToList();

        // We are done reading and can release the memory.
        PathChannel.Destroy(handle, length);
        return result;
    }
}
