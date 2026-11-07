using System.Collections;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents an iterator, which can be used to traverse over all attributes of an
///     <see cref="XmlElements.XmlElement" /> or <see cref="XmlTexts.XmlText" />.
/// </summary>
/// <remarks>
///     Check <see cref="NativeIterator" /> for how many times the iterator can be enumerated.
/// </remarks>
public class XmlAttributeIterator : NativeIterator, IEnumerable<KeyValuePair<string, string>>
{
    internal XmlAttributeIterator(Branch branch, Transaction transaction, CreateHandleFunction createHandle)
        : base(branch, transaction, createHandle, XmlAttributeChannel.IteratorDestroy)
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
}
