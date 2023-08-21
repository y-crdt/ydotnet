using YDotNet.Document.Types.Arrays.Events;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Document.Types.Texts.Events;
using YDotNet.Document.Types.XmlElements.Events;
using YDotNet.Document.Types.XmlTexts.Events;

namespace YDotNet.Document.Types.Events;

/// <summary>
///     Represents the type of event that this generic <see cref="EventBranch" /> holds.
/// </summary>
public enum EventBranchTag : sbyte
{
    /// <summary>
    ///     This event holds an <see cref="ArrayEvent" /> instance.
    /// </summary>
    Array = 1,

    /// <summary>
    ///     This event holds an <see cref="MapEvent" /> instance.
    /// </summary>
    Map = 2,

    /// <summary>
    ///     This event holds an <see cref="TextEvent" /> instance.
    /// </summary>
    Text = 3,

    /// <summary>
    ///     This event holds an <see cref="XmlElementEvent" /> instance.
    /// </summary>
    XmlElement = 4,

    /// <summary>
    ///     This event holds an <see cref="XmlTextEvent" /> instance.
    /// </summary>
    XmlText = 5
}
