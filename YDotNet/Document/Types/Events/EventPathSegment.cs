using System.Runtime.InteropServices;
using YDotNet.Infrastructure;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a segment of the full path represented by <see cref="EventPath" />.
/// </summary>
public class EventPathSegment
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventPathSegment" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    public EventPathSegment(nint handle)
    {
        Handle = handle;
        Tag = (EventPathSegmentTag)Marshal.ReadByte(handle);

        switch (Tag)
        {
            case EventPathSegmentTag.Key:
                var pointer = Marshal.ReadIntPtr(handle + MemoryConstants.PointerSize);
                Key = Marshal.PtrToStringAnsi(pointer);
                break;

            case EventPathSegmentTag.Index:
                Index = (uint)Marshal.ReadInt32(handle + MemoryConstants.PointerSize);
                break;
        }
    }

    /// <summary>
    ///     Gets the value that indicates the kind of data held by this <see cref="EventPathSegment" /> instance.
    /// </summary>
    public EventPathSegmentTag Tag { get; }

    /// <summary>
    ///     Gets the <see cref="string" /> key, if <see cref="Tag" /> is <see cref="EventPathSegmentTag.Key" />, or
    ///     <c>null</c> otherwise.
    /// </summary>
    public string? Key { get; }

    /// <summary>
    ///     Gets the <see cref="int" /> index, if <see cref="Tag" /> is <see cref="EventPathSegmentTag.Index" />, or
    ///     <c>null</c> otherwise.
    /// </summary>
    public uint? Index { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
