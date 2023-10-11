using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventDelta" /> instances.
/// </summary>
public class EventDeltas : UnmanagedCollectionResource<EventDelta>
{
    private readonly uint length;

    internal EventDeltas(nint handle, uint length, IResourceOwner owner)
        : base(handle, owner)
    {
        this.length = length;

        foreach (var itemHandle in MemoryReader.ReadIntPtrArray(handle, length, Marshal.SizeOf<EventDeltaNative>()))
        {
            // The event delta creates output that are owned by this block of allocated memory.
            AddItem(Marshal.PtrToStructure<EventDeltaNative>(itemHandle).ToEventDelta(this));
        }
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="EventDeltas"/> class.
    /// </summary>
    ~EventDeltas()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        EventChannel.DeltaDestroy(Handle, length);
    }
}
