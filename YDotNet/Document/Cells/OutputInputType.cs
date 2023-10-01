namespace YDotNet.Document.Cells;

/// <summary>
/// The type of an output.
/// </summary>
public enum OutputInputType
{
    /// <summary>
    /// No defined.
    /// </summary>
    NotSet = -99,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag boolean values.
    /// </summary>
    Bool = -8,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag floating point numbers.
    /// </summary>
    Double = -7,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag 64-bit integer numbers.
    /// </summary>
    Long = -6,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag strings.
    /// </summary>
    String = -5,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag binary content.
    /// </summary>
    Bytes = -4,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag embedded JSON-like arrays of values.
    /// </summary>
    Collection = -3,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag embedded JSON-like maps of key-value pairs.
    /// </summary>
    Object = -2,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag JSON-like null values.
    /// </summary>
    Null = -1,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag JSON-like undefined values.
    /// </summary>
    Undefined = 0,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YArray` shared type.
    /// </summary>
    Array = 1,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YMap` shared type.
    /// </summary>
    Map = 2,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YText` shared type.
    /// </summary>
    Text = 3,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YXmlElement` shared type.
    /// </summary>
    XmlElement = 4,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YXmlText` shared type.
    /// </summary>
    XmlText = 5,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YXmlFragment` shared type.
    /// </summary>
    XmlFragment = 6,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YDoc` shared type.
    /// </summary>
    Doc = 7
}
