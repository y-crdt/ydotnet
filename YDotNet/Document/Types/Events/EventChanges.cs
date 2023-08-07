using System.Collections;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventChange" /> instances.
/// </summary>
public class EventChanges : IEnumerable<EventChange>, IDisposable
{
    private readonly IEnumerable<EventChange> collection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventChanges" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="length">The length of the array of <see cref="EventChange" /> to read from <see cref="Handle" />.</param>
    public EventChanges(nint handle, uint length)
    {
        Handle = handle;
        Length = length;

        collection = MemoryReader.TryReadIntPtrArray(Handle, Length, Marshal.SizeOf<EventChangeNative>())!
            .Select(Marshal.PtrToStructure<EventChangeNative>)
            .Select(x => x.ToEventChange())
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
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    public IEnumerator<EventChange> GetEnumerator()
    {
        return collection.GetEnumerator();
    }
}
