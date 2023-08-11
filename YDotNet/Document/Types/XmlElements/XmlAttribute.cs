using System.Runtime.InteropServices;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     A structure representing single attribute of either an
///     <see cref="XmlElement" /> or <see cref="XmlText" /> instance.
/// </summary>
public class XmlAttribute : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlAttribute" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlAttribute(nint handle)
    {
        Handle = handle;

        // This offset is used to read subsequent pointers of the native resource.
        var offset = Marshal.SizeOf<nint>();

        Name = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(Handle));
        Value = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(Handle + offset));
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     The name of the attribute.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The value of the attribute.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        XmlAttributeChannel.Destroy(Handle);
    }
}
