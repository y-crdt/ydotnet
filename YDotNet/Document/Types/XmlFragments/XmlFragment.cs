using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlElements.Events;
using YDotNet.Document.Types.XmlElements.Trees;
using YDotNet.Document.Types.XmlFragments.Events;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlFragments;

/// <summary>
///     <para>
///         A shared data type that represents an untagged collection of XML nodes,
///         (<see cref="XmlElement" /> and <see cref="XmlText" />).
///     </para>
///     <para>
///         The <see cref="XmlFragment" /> is similar to <see cref="XmlElement" /> but
///         it doesn't have a tag or attributes.
///     </para>
/// </summary>
public class XmlFragment : Branch
{
    private readonly EventSubscriberFromId<XmlFragmentEvent> onObserve;

    internal XmlFragment(nint handle, Doc doc, bool isDeleted)
        : base(handle, doc, isDeleted)
    {
        onObserve = new EventSubscriberFromId<XmlFragmentEvent>(
            doc.EventManager,
            this,
            (xmlFragment, action) =>
            {
                XmlElementChannel.ObserveCallback callback = (_, eventHandle) =>
                    action(new XmlElementEvent(eventHandle, Doc));

                return (XmlElementChannel.Observe(xmlFragment, nint.Zero, callback), callback);
            },
            SubscriptionChannel.Unobserve);
    }

    /// <summary>
    ///     Returns a number of direct child nodes (both <see cref="XmlElement" /> and <see cref="XmlText" />)
    ///     of this <see cref="XmlFragment" />.
    /// </summary>
    /// <remarks>
    ///     This function doesn't count a recursive nodes, only direct children of this <see cref="XmlFragment" />.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The number of direct child nodes of this <see cref="XmlFragment" />.</returns>
    public uint ChildLength(Transaction transaction)
    {
        return XmlElementChannel.ChildLength(GetHandle(transaction), transaction.Handle);
    }

    /// <summary>
    ///     Inserts an <see cref="XmlText" /> as a child of this <see cref="XmlFragment" /> at the given
    ///     <paramref name="index" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index that the <see cref="XmlText" /> will be inserted.</param>
    /// <returns>The inserted <see cref="XmlText" /> at the given <paramref name="index" />.</returns>
    public XmlText InsertText(Transaction transaction, uint index)
    {
        var handle = XmlElementChannel.InsertText(GetHandle(transaction), transaction.Handle, index);

        return Doc.GetXmlText(handle, isDeleted: false);
    }

    /// <summary>
    ///     Inserts an <see cref="XmlElement" /> as a child of this <see cref="XmlFragment" /> at the given
    ///     <paramref name="index" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index that the <see cref="XmlText" /> will be inserted.</param>
    /// <param name="name">The name (or tag) of the <see cref="XmlElement" /> that will be inserted.</param>
    /// <returns>The inserted <see cref="XmlText" /> at the given <paramref name="index" />.</returns>
    public XmlElement InsertElement(Transaction transaction, uint index, string name)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = XmlElementChannel.InsertElement(GetHandle(transaction), transaction.Handle, index, unsafeName.Handle);

        return Doc.GetXmlElement(handle, isDeleted: false);
    }

    /// <summary>
    ///     Removes a consecutive range of direct child nodes starting at the <paramref name="index" /> through the
    ///     <paramref name="length" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index to start removing the child nodes.</param>
    /// <param name="length">The amount of child nodes to remove, starting at <paramref name="index" />.</param>
    public void RemoveRange(Transaction transaction, uint index, uint length)
    {
        XmlElementChannel.RemoveRange(GetHandle(transaction), transaction.Handle, index, length);
    }

    /// <summary>
    ///     Returns an <see cref="Output" /> cell or <c>null</c> if the <paramref name="index" /> is out of bounds.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index to retrieve the <see cref="Output" /> cell.</param>
    /// <returns>An <see cref="Output" /> cell or <c>null</c> if the <paramref name="index" /> is out of bounds.</returns>
    public Output? Get(Transaction transaction, uint index)
    {
        var handle = XmlElementChannel.Get(GetHandle(transaction), transaction.Handle, index);

        return handle != nint.Zero ? Output.CreateAndRelease(handle, Doc) : null;
    }

    /// <summary>
    ///     Returns the first child of the current <see cref="XmlFragment" /> node which can be an
    ///     <see cref="XmlElement" /> or an <see cref="XmlText" /> or <c>null</c> if this node is empty.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The first child of the current <see cref="XmlFragment" /> node which can be an
    ///     <see cref="XmlElement" /> or an <see cref="XmlText" /> or <c>null</c> if this node is empty.
    /// </returns>
    public Output? FirstChild(Transaction transaction)
    {
        var handle = XmlElementChannel.FirstChild(GetHandle(transaction), transaction.Handle);

        return handle != nint.Zero ? Output.CreateAndRelease(handle, Doc) : null;
    }

    /// <summary>
    ///     Returns an <see cref="XmlTreeWalker" /> for this <see cref="XmlFragment" />.
    /// </summary>
    /// <remarks>
    ///     Check the documentation of <see cref="XmlTreeWalker" /> for more information.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>An <see cref="XmlTreeWalker" /> for this <see cref="XmlFragment" />.</returns>
    public XmlTreeWalker TreeWalker(Transaction transaction)
    {
        var handle = XmlElementChannel.TreeWalker(GetHandle(transaction), transaction.Handle);

        return new XmlTreeWalker(handle.Checked(), Doc);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="XmlFragment" /> instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable Observe(Action<XmlFragmentEvent> action)
    {
        return onObserve.Subscribe(action);
    }
}
