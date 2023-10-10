using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the keys that changed the shared type that emitted the event related to this <see cref="EventKeys" />
///     instance.
/// </summary>
public class EventKeys : ReadOnlyCollection<EventKeyChange>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventKeys" /> class.
    /// </summary>
    /// <param name="handle">The handle to the beginning of the array of <see cref="EventKeyChangeNative" /> instances.</param>
    /// <param name="length">The length of the array.</param>
    internal EventKeys(nint handle, uint length)
        : base(ReadItems(handle, length))
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    private static IList<EventKeyChange> ReadItems(nint handle, uint length)
    {
        var result = MemoryReader.ReadIntPtrArray(handle, length, Marshal.SizeOf<EventKeyChangeNative>())
            .Select(Marshal.PtrToStructure<EventKeyChangeNative>)
            .Select(x => x.ToEventKeyChange())
            .ToList();

        // We are done reading and can release the memory.
        EventChannel.KeysDestroy(handle, length);
        return result;
    }
}
