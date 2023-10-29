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
public class XmlAttributeIterator : UnmanagedResource, IEnumerable<KeyValuePair<string, string>>
{
    internal XmlAttributeIterator(nint handle)
        : base(handle)
    {
    }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return new XmlAttributeEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="XmlAttributeIterator" /> class.
    /// </summary>
    ~XmlAttributeIterator()
    {
        Dispose(disposing: false);
    }

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        XmlAttributeChannel.IteratorDestroy(Handle);
    }
}
