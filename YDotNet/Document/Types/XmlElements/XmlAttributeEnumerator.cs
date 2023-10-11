using System.Collections;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents the iterator to provide instances of <see cref="XmlAttribute" /> or <c>null</c> to
///     <see cref="XmlAttributeIterator" />.
/// </summary>
internal class XmlAttributeEnumerator : IEnumerator<XmlAttribute>
{
    private XmlAttribute? current;

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

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public XmlAttribute Current => current!;

    /// <inheritdoc />
    object? IEnumerator.Current => current!;

    private XmlAttributeIterator Iterator { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = XmlAttributeChannel.IteratorNext(Iterator.Handle);

        current = handle != nint.Zero ? new XmlAttribute(handle) : null;

        return current != null;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
