namespace YDotNet.Document.Cells;

/// <summary>
/// The type of an output.
/// </summary>
public enum OutputInputType
{
    /// <summary>
    /// No defined.
    /// </summary>
    Undefined = -99,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag boolean values.
    /// </summary>
    JsonBool = -8,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag floating point numbers.
    /// </summary>
    JsonNumber = -7,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag 64-bit integer numbers.
    /// </summary>
    JsonInteger = -6,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag strings.
    /// </summary>
    JsonString = -5,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag binary content.
    /// </summary>
    JsonBuffer = -4,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag embedded JSON-like arrays of values.
    /// </summary>
    JsonArray = -3,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag embedded JSON-like maps of key-value pairs.
    /// </summary>
    JsonMap = -2,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag JSON-like null values.
    /// </summary>
    JsonNull = -1,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag JSON-like undefined values.
    /// </summary>
    JsonUndefined = 0,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YArray` shared type.
    /// </summary>
    YArray = 1,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YMap` shared type.
    /// </summary>
    YMap = 2,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YText` shared type.
    /// </summary>
    YText = 3,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YXmlElement` shared type.
    /// </summary>
    YXmlElement = 4,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YXmlText` shared type.
    /// </summary>
    YXmlText = 5,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YXmlFragment` shared type.
    /// </summary>
    YXmlFragment = 6,

    /// <summary>
    /// Flag used by `YInput` and `YOutput` to tag content, which is an `YDoc` shared type.
    /// </summary>
    YDoc = 7
}
