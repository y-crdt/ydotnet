using System.Collections;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents an iterator, which can be used to traverse over all attributes of an <see cref="XmlElement" /> or
///     <see cref="XmlText" />.
/// </summary>
/// <remarks>
///     The iterator can't be reused. If needed, use <see cref="Enumerable.ToArray{TSource}" /> to accumulate values.
/// </remarks>
public class XmlAttributeIterator : IEnumerable<XmlAttribute>, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlAttributeIterator" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlAttributeIterator(nint handle)
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
