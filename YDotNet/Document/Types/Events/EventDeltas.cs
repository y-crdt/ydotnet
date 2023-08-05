using System.Collections;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventDelta" /> instances.
/// </summary>
public class EventDeltas : IEnumerable<EventDelta>, IDisposable
{
    private readonly IEnumerable<EventDelta> collection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventDeltas" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="length">The length of the array of <see cref="EventDelta" /> to read from <see cref="Handle" />.</param>
    public EventDeltas(nint handle, uint length)
    {
        Handle = handle;
        Length = length;

        collection = MemoryReader.TryReadIntPtrArray(Handle, Length, Marshal.SizeOf<EventDeltaNative>())!
            .Select(Marshal.PtrToStructure<EventDeltaNative>)
            .Select(x => x.ToEventDelta())
            .ToArray();
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the length of the native resource.
    /// </summary>
    internal uint Length { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        EventChannel.DeltaDestroy(Handle, Length);
    }

    /// <inheritdoc />
    public IEnumerator<EventDelta> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
