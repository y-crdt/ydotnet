using System.Collections;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents the iterator to provide instances of <see cref="XmlAttribute" /> or <c>null</c> to
///     <see cref="XmlAttributeIterator" />.
/// </summary>
internal class XmlAttributeEnumerator : IEnumerator<XmlAttribute>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlAttributeEnumerator" /> class.
    /// </summary>
    /// <param name="iterator">
    ///     The <see cref="Iterator" /> instance used by this enumerator.
    ///     Check <see cref="Iterator" /> for more details.
    /// </param>
    internal XmlAttributeEnumerator(XmlAttributeIterator iterator)
    {
        Iterator = iterator;
    }

    /// <summary>
    ///     Gets the <see cref="Iterator" /> instance that holds the
    ///     <see cref="XmlAttributeIterator.Handle" /> used by this enumerator.
    /// </summary>
    private XmlAttributeIterator Iterator { get; }

    /// <inheritdoc />
    public XmlAttribute? Current { get; private set; }

    /// <inheritdoc />
    object? IEnumerator.Current => Current;

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = XmlAttributeChannel.IteratorNext(Iterator.Handle);

        Current = handle != nint.Zero ? new XmlAttribute(handle) : null;

        return Current != null;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Iterator.Dispose();
    }
}
