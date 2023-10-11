using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the keys that changed the shared type that emitted the event related to this <see cref="EventKeys" />
///     instance.
/// </summary>
public class EventKeys : UnmanagedCollectionResource<EventKeyChange>
{
    private readonly uint length;

    internal EventKeys(nint handle, uint length, IResourceOwner owner)
        : base(handle, owner)
    {
        foreach (var keyHandle in MemoryReader.ReadIntPtrArray(handle, length, Marshal.SizeOf<EventKeyChangeNative>()))
        {
            // The event delta creates output that are owned by this block of allocated memory.
            AddItem(Marshal.PtrToStructure<EventKeyChangeNative>(keyHandle).ToEventKeyChange(this));
        }

        this.length = length;
    }

    ~EventKeys()
    {
        Dispose(true);
    }

    protected override void DisposeCore(bool disposing)
    {
        EventChannel.KeysDestroy(Handle, length);
    }
}
