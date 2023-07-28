using System.Collections;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the keys that changed the shared type that emitted the event related to this <see cref="EventKeys" />
///     instance.
/// </summary>
public class EventKeys : IEnumerable<EventKeyChange>, IDisposable
{
    private readonly IEnumerable<EventKeyChange> collection;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EventKeys" /> class.
    /// </summary>
    /// <param name="handle">The handle to the beginning of the array of <see cref="EventKeyChangeNative" /> instances.</param>
    /// <param name="length">The length of the array.</param>
    internal EventKeys(nint handle, uint length)
    {
        Handle = handle;
        Length = length;

        collection = MemoryReader.TryReadIntPtrArray(Handle, Length, Marshal.SizeOf<EventKeyChangeNative>())!
            .Select(Marshal.PtrToStructure<EventKeyChangeNative>)
            .Select(x => x.ToEventKeyChange())
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
        EventChannel.Destroy(Handle, Length);
    }

    /// <inheritdoc />
    public IEnumerator<EventKeyChange> GetEnumerator()
    {
        return collection.GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
