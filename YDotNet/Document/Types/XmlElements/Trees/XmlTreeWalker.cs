using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.XmlFragments;
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
///     Check <see cref="NativeIterator" /> for how many times the tree walker can be enumerated.
/// </remarks>
public class XmlTreeWalker : NativeIterator, IEnumerable<Output>
{
    internal XmlTreeWalker(XmlFragment xmlFragment, Transaction transaction)
        : base(xmlFragment, transaction, XmlElementChannel.TreeWalker, XmlElementChannel.TreeWalkerDestroy)
    {
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
