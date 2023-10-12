using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a collection of <see cref="EventChange" /> instances.
/// </summary>
public class EventChanges : UnmanagedCollectionResource<EventChange>
{
    private readonly uint length;

    internal EventChanges(nint handle, uint length, Doc doc, IResourceOwner owner)
        : base(handle, owner)
    {
        foreach (var itemHandle in MemoryReader.ReadIntPtrArray(handle, length, Marshal.SizeOf<EventChangeNative>()))
        {
            // The event delta creates output that are owned by this block of allocated memory.
            AddItem(Marshal.PtrToStructure<EventChangeNative>(itemHandle).ToEventChange(this, doc));
        }

        this.length = length;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="EventChanges"/> class.
    /// </summary>
    ~EventChanges()
    {
        Dispose(true);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        EventChannel.DeltaDestroy(Handle, length);
    }
}
