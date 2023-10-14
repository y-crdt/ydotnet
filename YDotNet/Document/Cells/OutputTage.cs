namespace YDotNet.Document.Cells;

/// <summary>
/// The type of an output.
/// </summary>
public enum OutputTag
{
    /// <summary>
    /// No defined.
    /// </summary>
    NotSet = -99,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag boolean values.
    /// </summary>
    Bool = -8,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag floating point numbers.
    /// </summary>
    Double = -7,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag 64-bit integer numbers.
    /// </summary>
    Long = -6,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag strings.
    /// </summary>
    String = -5,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag binary content.
    /// </summary>
    Bytes = -4,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag embedded JSON-like arrays of values.
    /// </summary>
    JsonArray = -3,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag embedded JSON-like maps of key-value pairs.
    /// </summary>
    JsonObject = -2,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag JSON-like null values.
    /// </summary>
    Null = -1,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag JSON-like undefined values.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YArray` shared type.
    /// </summary>
    Array = 1,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YMap` shared type.
    /// </summary>
    Map = 2,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YText` shared type.
    /// </summary>
    Text = 3,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YXmlElement` shared type.
    /// </summary>
    XmlElement = 4,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YXmlText` shared type.
    /// </summary>
    XmlText = 5,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YXmlFragment` shared type.
    /// </summary>
    XmlFragment = 6,

    /// <summary>
    /// Flag used by <see cref="Output" /> to tag content, which is an `YDoc` shared type.
    /// </summary>
    Doc = 7
}
