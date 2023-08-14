using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements.Trees;

/// <summary>
///     Returns an iterator over a nested recursive nodes of an <see cref="XmlElement" />.
/// </summary>
/// <remarks>
///     The <see cref="XmlTreeWalker" /> traverses values using depth-first and nodes can be either
///     <see cref="XmlElement" /> or <see cref="XmlText" /> nodes.
/// </remarks>
public class XmlTreeWalker : IEnumerable<Output>, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlTreeWalker" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    public XmlTreeWalker(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        XmlElementChannel.TreeWalkerDestroy(Handle);
    }

    /// <inheritdoc />
    public IEnumerator<Output> GetEnumerator()
    {
        return new XmlTreeWalkerEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
