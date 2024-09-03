using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Document.Types.Texts.Events;
using YDotNet.Document.Types.XmlElements.Events;
using YDotNet.Document.Types.XmlFragments.Events;
using YDotNet.Document.Types.XmlTexts.Events;
using YDotNet.Native;
using YDotNet.Native.Document.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents a generic container to hold events that happen on shared types.
///     Check the <see cref="Tag" /> to know which of the event types will be available at runtime.
/// </summary>
public class EventBranch
{
    private readonly object? value;

    internal EventBranch(NativeWithHandle<EventBranchNative> native, Doc doc)
    {
        Tag = (EventBranchTag) native.Value.Tag;

        value = BuildValue(native, doc, Tag);
    }

    /// <summary>
    ///     Gets the value that indicates the kind of event held by this <see cref="EventBranch" /> instance.
    /// </summary>
    public EventBranchTag Tag { get; }

    /// <summary>
    ///     Gets the <see cref="MapEvent" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public MapEvent MapEvent => GetValue<MapEvent>(EventBranchTag.Map);

    /// <summary>
    ///     Gets the <see cref="TextEvent" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public TextEvent TextEvent => GetValue<TextEvent>(EventBranchTag.Text);

    /// <summary>
    ///     Gets the <see cref="ArrayEvent" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public ArrayEvent ArrayEvent => GetValue<ArrayEvent>(EventBranchTag.Array);

    /// <summary>
    ///     Gets the <see cref="XmlElementEvent" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public XmlElementEvent XmlElementEvent => GetValue<XmlElementEvent>(EventBranchTag.XmlElement);

    /// <summary>
    ///     Gets the <see cref="XmlTextEvent" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public XmlTextEvent XmlTextEvent => GetValue<XmlTextEvent>(EventBranchTag.XmlText);

    /// <summary>
    ///     Gets the <see cref="XmlFragmentEvent" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public XmlFragmentEvent XmlFragmentEvent => GetValue<XmlFragmentEvent>(EventBranchTag.XmlFragment);

    private static object? BuildValue(NativeWithHandle<EventBranchNative> native, Doc doc, EventBranchTag tag)
    {
        var handle = native.Value.ValueHandle(native.Handle);

        return tag switch
        {
            EventBranchTag.Map => new MapEvent(handle, doc),
            EventBranchTag.Text => new TextEvent(handle, doc),
            EventBranchTag.Array => new ArrayEvent(handle, doc),
            EventBranchTag.XmlElement => new XmlElementEvent(handle, doc),
            EventBranchTag.XmlText => new XmlTextEvent(handle, doc),
            EventBranchTag.XmlFragment => new XmlFragmentEvent(handle, doc),
            _ => null
        };
    }

    private T GetValue<T>(EventBranchTag expectedType)
    {
        if (value?.GetType() != typeof(T))
        {
            throw new YDotNetException($"Expected {expectedType}, got {Tag}.");
        }

        return (T) value;
    }
}
