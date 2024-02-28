namespace YDotNet.Document.Cells;

/// <summary>
///     Represents the type of value stored inside of an <see cref="Output" /> cell.
/// </summary>
/// <remarks>
///     This can be used as a safety check to only read the correct type of value from the cell.
/// </remarks>
public enum OutputTag
{
    /// <summary>
    ///     Represents a cell with a <see cref="bool" /> value.
    /// </summary>
    Boolean = -8,

    /// <summary>
    ///     Represents a cell with a <see cref="double" /> value.
    /// </summary>
    Double = -7,

    /// <summary>
    ///     Represents a cell with a <see cref="long" /> value.
    /// </summary>
    Long = -6,

    /// <summary>
    ///     Represents a cell with a <see cref="string" /> value.
    /// </summary>
    String = -5,

    /// <summary>
    ///     Represents a cell with a <see cref="byte" /> array value.
    /// </summary>
    Bytes = -4,

    /// <summary>
    ///     Represents a cell with an array of <see cref="Input" /> value.
    /// </summary>
    JsonArray = -3,

    /// <summary>
    ///     Represents a cell with a dictionary of <see cref="Input" /> value.
    /// </summary>
    JsonObject = -2,

    /// <summary>
    ///     Represents a cell with the <c>null</c> value.
    /// </summary>
    Null = -1,

    /// <summary>
    ///     Represents a cell with the <c>undefined</c> value.
    /// </summary>
    Undefined = 0,

    /// <summary>
    ///     Represents a cell with an <see cref="YDotNet.Document.Types.Arrays.Array" /> value.
    /// </summary>
    Array = 1,

    /// <summary>
    ///     Represents a cell with an <see cref="YDotNet.Document.Types.Maps.Map" /> value.
    /// </summary>
    Map = 2,

    /// <summary>
    ///     Represents a cell with an <see cref="YDotNet.Document.Types.Texts.Text" /> value.
    /// </summary>
    Text = 3,

    /// <summary>
    ///     Represents a cell with an <see cref="YDotNet.Document.Types.XmlElements.XmlElement" /> value.
    /// </summary>
    XmlElement = 4,

    /// <summary>
    ///     Represents a cell with an <see cref="YDotNet.Document.Types.XmlTexts.XmlText" /> value.
    /// </summary>
    XmlText = 5,

    // The following constant is commented because it's not exposed by `Input` or `Output`.
    // XmlFragment = 6,

    /// <summary>
    ///     Represents a cell with an <see cref="YDotNet.Document.Doc" /> value.
    /// </summary>
    Doc = 7,
}
