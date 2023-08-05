using System.Runtime.InteropServices;
using YDotNet.Document.Types.Branches;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents a XML element.
/// </summary>
public class XmlElement : Branch, IDisposable
{
    private nint? tagHandle;

    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlElement" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlElement(nint handle)
        : base(handle)
    {
        // Nothing here.
    }

    /// <summary>
    ///     Gets the name (or tag) of the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     Root-level XML nodes use "UNDEFINED" as their tag names.
    /// </remarks>
    public string Tag
    {
        get
        {
            if (!tagHandle.HasValue)
            {
                tagHandle = XmlElementChannel.Tag(Handle);
            }

            return Marshal.PtrToStringAnsi(tagHandle.Value);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (tagHandle.HasValue)
        {
            StringChannel.Destroy(tagHandle.Value);
        }
    }
}
