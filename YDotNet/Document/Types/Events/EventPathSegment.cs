using System.Runtime.InteropServices;

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
        Tag = (EventPathSegmentTag) Marshal.ReadByte(handle);

        // Offset is the size of an `nint` because the C struct contains two fields. They're 1-byte and 8-byte long
        // and due to memory alignment, the 8-byte field is put after 7 empty bytes after the first field.
        var offset = Marshal.SizeOf<nint>();

        switch (Tag)
        {
            case EventPathSegmentTag.Key:
                var pointer = Marshal.ReadIntPtr(handle + offset);
                Key = Marshal.PtrToStringAnsi(pointer);
                break;

            case EventPathSegmentTag.Index:
                Index = (uint) Marshal.ReadInt32(handle + offset);
                break;
        }
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

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
}
