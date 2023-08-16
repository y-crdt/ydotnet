namespace YDotNet.Document.Options;

/// <summary>
///     Determines how string length and offsets are calculated for <see cref="Text" /> and <see cref="XmlText" />.
/// </summary>
public enum DocEncoding
{
    /// <summary>
    ///     Compute editable strings length and offset using UTF-8 byte count.
    /// </summary>
    Utf8 = 1,

    /// <summary>
    ///     Compute editable strings length and offset using UTF-16 chars count.
    /// </summary>
    Utf16 = 2,

    /// <summary>
    ///     Compute editable strings length and offset using UTF-32 (Unicode) code points number.
    /// </summary>
    Utf32 = 3
}
