using System.Runtime.InteropServices;
using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Document.Types.Texts.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a generic container to hold events that happen on shared types.
///     Check the <see cref="Tag" /> to know which of the event types will be available at runtime.
/// </summary>
public class EventBranch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EventBranch" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    public EventBranch(nint handle)
    {
        Handle = handle;
        Tag = (EventBranchTag) Marshal.ReadByte(handle);

        // Offset is the size of an `nint` because the C struct contains two fields. They're 1-byte and 8-byte long
        // and due to memory alignment, the 8-byte field is put after 7 empty bytes after the first field.
        var offset = Marshal.SizeOf<nint>();

        switch (Tag)
        {
            case EventBranchTag.Map:
                MapEvent = new MapEvent(handle + offset);
                break;
            case EventBranchTag.Text:
                TextEvent = new TextEvent(handle + offset);
                break;
            case EventBranchTag.Array:
                ArrayEvent = new ArrayEvent(handle + offset);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Gets the value that indicates the kind of event held by this <see cref="EventBranch" /> instance.
    /// </summary>
    public EventBranchTag Tag { get; }

    /// <summary>
    ///     Gets the <see cref="MapEvent" />, if <see cref="Tag" /> is <see cref="EventBranchTag.Map" />, or <c>null</c>
    ///     otherwise.
    /// </summary>
    public MapEvent? MapEvent { get; }

    /// <summary>
    ///     Gets the <see cref="TextEvent" />, if <see cref="Tag" /> is <see cref="EventBranchTag.Text" />, or <c>null</c>
    ///     otherwise.
    /// </summary>
    public TextEvent? TextEvent { get; }

    /// <summary>
    ///     Gets the <see cref="ArrayEvent" />, if <see cref="Tag" /> is <see cref="EventBranchTag.Array" />, or <c>null</c>
    ///     otherwise.
    /// </summary>
    public ArrayEvent? ArrayEvent { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
