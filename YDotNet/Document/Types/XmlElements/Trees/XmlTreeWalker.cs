using System.Collections;
using YDotNet.Document.Cells;
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
    internal XmlTreeWalker(nint handle)
        : base(handle)
    {
    }

    ~XmlTreeWalker()
    {
        Dispose(false);
    }

    protected override void DisposeCore(bool disposing)
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
