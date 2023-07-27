using System.Collections;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the path from the root type to the shared type that emitted the event related to this
///     <see cref="EventPath" /> instance.
/// </summary>
public class EventPath : IEnumerable<EventPathSegment>, IDisposable
{
    private readonly IEnumerable<EventPathSegment> collection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventPath" /> class.
    /// </summary>
    /// <param name="handle">The handle to the beginning of the array of <see cref="EventPathSegment" /> instances.</param>
    /// <param name="length">The length of the array.</param>
    internal EventPath(nint handle, uint length)
    {
        Handle = handle;
        Length = length;

        collection = MemoryReader.TryReadIntPtrArray(Handle, Length, size: 16)!
            .Select(x => new EventPathSegment(x))
            .ToArray();
    }

    /// <summary>
    ///     Gets the length of the native resource.
    /// </summary>
    internal uint Length { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        PathChannel.Destroy(Handle, Length);
    }

    /// <inheritdoc />
    public IEnumerator<EventPathSegment> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
