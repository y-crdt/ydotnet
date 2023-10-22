using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements.Trees;

/// <summary>
///     Returns an iterator over a nested recursive nodes of an <see cref="XmlElement" />.
/// </summary>
/// <remarks>
///     The <see cref="XmlTreeWalker" /> traverses values using depth-first and nodes can be either
///     <see cref="XmlElement" /> or <see cref="XmlText" /> nodes.
/// </remarks>
public class XmlTreeWalker : UnmanagedResource, IEnumerable<Output>
{
    internal XmlTreeWalker(nint handle, Doc doc)
        : base(handle)
    {
        Doc = doc;
    }

    internal Doc Doc { get; }

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

    /// <summary>
    ///     Finalizes an instance of the <see cref="XmlTreeWalker" /> class.
    /// </summary>
    ~XmlTreeWalker()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    protected internal override void DisposeCore(bool disposing)
    {
        XmlElementChannel.TreeWalkerDestroy(Handle);
    }
}
