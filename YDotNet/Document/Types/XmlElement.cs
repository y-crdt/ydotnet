using YDotNet.Native.Types;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents a XML element.
/// </summary>
public class XmlElement
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlElement" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlElement(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the name (or tag) of the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     Root-level XML nodes use "UNDEFINED" as their tag names.
    /// </remarks>
    // TODO [LSViana] Release the value returned here with `ystring_destroy()` later.
    public string Tag => XmlElementChannel.Tag(Handle);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
