using System.Collections;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the keys that changed in an event that happened in the parent instance.
/// </summary>
public class EventKeys : IEnumerable<EventKeyChange>, IDisposable
{
    private readonly IEnumerable<EventKeyChange> collection;

    internal EventKeys(nint handle, uint length)
    {
        Handle = handle;
        Length = length;

        collection = MemoryReader.TryReadIntPtrArray(Handle, Length, Marshal.SizeOf<EventKeyChangeNative>())
            ?.Select(Marshal.PtrToStructure<EventKeyChangeNative>)
            .Select(x => x.ToEventKeyChange())
            .ToArray();
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    public uint Length { get; }

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
