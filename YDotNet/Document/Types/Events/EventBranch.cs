using System.Runtime.InteropServices;
using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Document.Types.Texts.Events;
using YDotNet.Document.Types.XmlElements.Events;
using YDotNet.Document.Types.XmlTexts.Events;
using YDotNet.Infrastructure;

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

        switch (Tag)
        {
            case EventBranchTag.Map:
                MapEvent = new MapEvent(handle + MemoryConstants.PointerSize);
                break;
            case EventBranchTag.Text:
                TextEvent = new TextEvent(handle + MemoryConstants.PointerSize);
                break;
            case EventBranchTag.Array:
                ArrayEvent = new ArrayEvent(handle + MemoryConstants.PointerSize);
                break;
            case EventBranchTag.XmlElement:
                XmlElementEvent = new XmlElementEvent(handle + MemoryConstants.PointerSize);
                break;
            case EventBranchTag.XmlText:
                XmlTextEvent = new XmlTextEvent(handle + MemoryConstants.PointerSize);
                break;
            default:
                throw new NotSupportedException(
                    $"The value \"{Tag}\" is not supported by the {nameof(EventBranch)} class.");
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
    ///     Gets the <see cref="XmlElementEvent" />, if <see cref="Tag" /> is <see cref="EventBranchTag.XmlElement" />,
    ///     or <c>null</c> otherwise.
    /// </summary>
    public XmlElementEvent? XmlElementEvent { get; }

    /// <summary>
    ///     Gets the <see cref="XmlTextEvent" />, if <see cref="Tag" /> is <see cref="EventBranchTag.XmlText" />,
    ///     or <c>null</c> otherwise.
    /// </summary>
    public XmlTextEvent? XmlTextEvent { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
