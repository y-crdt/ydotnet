using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Document.Types.Texts.Events;
using YDotNet.Document.Types.XmlElements.Events;
using YDotNet.Document.Types.XmlFragments.Events;
using YDotNet.Document.Types.XmlTexts.Events;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the type of event that this generic <see cref="EventBranch" /> holds.
/// </summary>
public enum EventBranchTag : sbyte
{
    /// <summary>
    ///     This event holds an <see cref="ArrayEvent" /> instance.
    /// </summary>
    Array = BranchKind.Array,

    /// <summary>
    ///     This event holds an <see cref="MapEvent" /> instance.
    /// </summary>
    Map = BranchKind.Map,

    /// <summary>
    ///     This event holds an <see cref="TextEvent" /> instance.
    /// </summary>
    Text = BranchKind.Text,

    /// <summary>
    ///     This event holds an <see cref="XmlElementEvent" /> instance.
    /// </summary>
    XmlElement = BranchKind.XmlElement,

    /// <summary>
    ///     This event holds an <see cref="XmlTextEvent" /> instance.
    /// </summary>
    XmlText = BranchKind.XmlText,

    /// <summary>
    ///     This event holds an <see cref="XmlFragmentEvent" /> instance.
    /// </summary>
    XmlFragment = BranchKind.XmlFragment
}
