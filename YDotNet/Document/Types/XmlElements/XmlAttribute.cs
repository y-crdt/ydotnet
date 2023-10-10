using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     A structure representing single attribute of either an <see cref="XmlElement" /> or <see cref="XmlText" /> instance.
/// </summary>
public class XmlAttribute
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlAttribute" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlAttribute(nint handle)
    {
        Handle = handle;

        Name = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(Handle)) ??
            throw new YDotNetException("Failed to read name.");

        Value = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(Handle + MemoryConstants.PointerSize)) ??
            throw new YDotNetException("Failed to read value.");

        // We are done reading and can release the memory.
        XmlAttributeChannel.Destroy(Handle);
    }

    /// <summary>
    ///     Gets the name of the attribute.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets the value of the attribute.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
