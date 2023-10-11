using System.Collections;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents an iterator, which can be used to traverse over all attributes of an
///     <see cref="XmlElements.XmlElement" /> or <see cref="XmlTexts.XmlText" />.
/// </summary>
/// <remarks>
///     The iterator can't be reused. If needed, use <see cref="Enumerable.ToArray{TSource}" /> to accumulate values.
/// </remarks>
public class XmlAttributeIterator : UnmanagedResource, IEnumerable<XmlAttribute>
{
    internal XmlAttributeIterator(nint handle)
        : base(handle)
    {
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="XmlAttributeIterator"/> class.
    /// </summary>
    ~XmlAttributeIterator()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        XmlAttributeChannel.IteratorDestroy(Handle);
    }

    /// <inheritdoc />
    public IEnumerator<XmlAttribute> GetEnumerator()
    {
        return new XmlAttributeEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
