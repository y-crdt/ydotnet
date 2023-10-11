using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the path from the root type to the shared type that emitted the event related to this
///     <see cref="EventPath" /> instance.
/// </summary>
public sealed class EventPath : UnmanagedCollectionResource<EventPathSegment>
{
    internal EventPath(nint handle, uint length, IResourceOwner owner)
        : base(handle, owner)
    {
        foreach (var itemHandle in MemoryReader.ReadIntPtrArray(Handle, length, size: 16))
        {
            // The segment does not make any further allocations.
            AddItem(new EventPathSegment(itemHandle));
        }

        // We have read everything, so we can release the memory immediately.
        PathChannel.Destroy(Handle, length);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        // We have read everything in the constructor, therefore there is no unmanaged memory to be released.
    }
}
