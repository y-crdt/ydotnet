using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.Types.XmlElements.Events;
using YDotNet.Document.Types.XmlElements.Trees;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     A shared data type that represents an XML element.
/// </summary>
public class XmlElement : Branch
{
    private readonly EventSubscriptions subscriptions = new();

    internal XmlElement(nint handle)
        : base(handle)
    {
    }

    /// <summary>
    ///     Gets the name (or tag) of the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     This property returns <c>null</c> for root-level XML nodes.
    /// </remarks>
    public string? Tag
    {
        get
        {
            var handle = XmlElementChannel.Tag(Handle);

            return handle != nint.Zero ? MemoryReader.ReadStringAndDestroy(handle) : null;
        }
    }

    /// <summary>
    ///     Returns the string representation of the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     The returned value has no padding or indentation spaces.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The string representation of the <see cref="XmlElement" /> instance.</returns>
    public string String(Transaction transaction)
    {
        var handle = XmlElementChannel.String(Handle, transaction.Handle);

        return MemoryReader.ReadStringAndDestroy(handle.Checked());
    }

    /// <summary>
    ///     Inserts an attribute in this <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     If another attribute with the same <paramref name="name" /> already exists, it will be replaced.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be added.</param>
    /// <param name="value">The value of the attribute to be added.</param>
    public void InsertAttribute(Transaction transaction, string name, string value)
    {
        using var unsageName = MemoryWriter.WriteUtf8String(name);
        using var unsafeValue = MemoryWriter.WriteUtf8String(value);

        XmlElementChannel.InsertAttribute(Handle, transaction.Handle, unsageName.Handle, unsafeValue.Handle);
    }

    /// <summary>
    ///     Removes an attribute from this <see cref="XmlElement" /> instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be removed.</param>
    public void RemoveAttribute(Transaction transaction, string name)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        XmlElementChannel.RemoveAttribute(Handle, transaction.Handle, unsafeName.Handle);
    }

    /// <summary>
    ///     Gets an attribute with the given <paramref name="name" /> from this <see cref="XmlElement" /> instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be retrieved.</param>
    /// <returns>The value of the attribute or <c>null</c> if it doesn't exist.</returns>
    public string? GetAttribute(Transaction transaction, string name)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = XmlElementChannel.GetAttribute(Handle, transaction.Handle, unsafeName.Handle);

        return handle != nint.Zero ? MemoryReader.ReadStringAndDestroy(handle) : null;
    }

    /// <summary>
    ///     Returns a <see cref="XmlAttributeIterator" />, which can be used to traverse
    ///     over all attributes of this <see cref="XmlElement" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The <see cref="XmlAttributeIterator" /> instance or <c>null</c> if failed.</returns>
    public XmlAttributeIterator Iterate(Transaction transaction)
    {
        var handle = XmlElementChannel.AttributeIterator(Handle, transaction.Handle);

        return new XmlAttributeIterator(handle.Checked());
    }

    /// <summary>
    ///     Returns a number of direct child nodes (both <see cref="XmlElement" /> and <see cref="XmlText" />)
    ///     of this <see cref="XmlElement" />.
    /// </summary>
    /// <remarks>
    ///     This function doesn't count a recursive nodes, only direct children of this <see cref="XmlElement" />.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The number of direct child nodes of this <see cref="XmlElement" />.</returns>
    public uint ChildLength(Transaction transaction)
    {
        return XmlElementChannel.ChildLength(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Inserts an <see cref="XmlText" /> as a child of this <see cref="XmlElement" /> at the given
    ///     <paramref name="index" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index that the <see cref="XmlText" /> will be inserted.</param>
    /// <returns>The inserted <see cref="XmlText" /> at the given <paramref name="index" />.</returns>
    public XmlText InsertText(Transaction transaction, uint index)
    {
        var handle = XmlElementChannel.InsertText(Handle, transaction.Handle, index);

        return new XmlText(handle.Checked());
    }

    /// <summary>
    ///     Inserts an <see cref="XmlText" /> as a child of this <see cref="XmlElement" /> at the given
    ///     <paramref name="index" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index that the <see cref="XmlText" /> will be inserted.</param>
    /// <param name="name">The name (or tag) of the <see cref="XmlElement" /> that will be inserted.</param>
    /// <returns>The inserted <see cref="XmlText" /> at the given <paramref name="index" />.</returns>
    public XmlElement InsertElement(Transaction transaction, uint index, string name)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var elementHandle = XmlElementChannel.InsertElement(Handle, transaction.Handle, index, unsafeName.Handle);

        return new XmlElement(elementHandle.Checked());
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
        XmlElementChannel.RemoveRange(Handle, transaction.Handle, index, length);
    }

    /// <summary>
    ///     Returns an <see cref="Output" /> cell or <c>null</c> if the <paramref name="index" /> is out of bounds.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The index to retrieve the <see cref="Output" /> cell.</param>
    /// <returns>An <see cref="Output" /> cell or <c>null</c> if the <paramref name="index" /> is out of bounds.</returns>
    public Output? Get(Transaction transaction, uint index)
    {
        var handle = XmlElementChannel.Get(Handle, transaction.Handle, index);

        return handle != nint.Zero ? new Output(handle, null) : null;
    }

    /// <summary>
    ///     Returns the previous sibling of this <see cref="XmlElement" /> instance which can be either an
    ///     <see cref="XmlElement" />
    ///     or an <see cref="XmlText" /> or <c>null</c> if this node is the first child.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The previous sibling of this <see cref="XmlElement" /> instance which can be either an
    ///     <see cref="XmlElement" /> or an <see cref="XmlText" /> or <c>null</c> if this node is the first child.
    /// </returns>
    public Output? PreviousSibling(Transaction transaction)
    {
        var handle = XmlChannel.PreviousSibling(Handle, transaction.Handle);

        return handle != nint.Zero ? new Output(handle, null) : null;
    }

    /// <summary>
    ///     Returns the next sibling of this <see cref="XmlElement" /> instance which can be either an
    ///     <see cref="XmlElement" />
    ///     or an <see cref="XmlText" /> or <c>null</c> if this node is the last child.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The next sibling of this <see cref="XmlElement" /> instance which can be either an <see cref="XmlElement" />
    ///     or an <see cref="XmlText" /> or <c>null</c> if this node is the last child.
    /// </returns>
    public Output? NextSibling(Transaction transaction)
    {
        var handle = XmlChannel.NextSibling(Handle, transaction.Handle);

        return handle != nint.Zero ? new Output(handle, null) : null;
    }

    /// <summary>
    ///     Returns the first child of the current <see cref="XmlElement" /> node which can be an
    ///     <see cref="XmlElement" /> or an <see cref="XmlText" /> or <c>null</c> if this node is empty.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The first child of the current <see cref="XmlElement" /> node which can be an
    ///     <see cref="XmlElement" /> or an <see cref="XmlText" /> or <c>null</c> if this node is empty.
    /// </returns>
    public Output? FirstChild(Transaction transaction)
    {
        var handle = XmlElementChannel.FirstChild(Handle, transaction.Handle);

        return handle != nint.Zero ? new Output(handle, null) : null;
    }

    /// <summary>
    ///     Returns the parent <see cref="XmlElement" /> of the current <see cref="XmlElement" /> node or
    ///     <c>null</c> if the current node is root-level node.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The parent <see cref="XmlElement" /> of the current <see cref="XmlElement" /> node or
    ///     <c>null</c> if the current node is root-level node.
    /// </returns>
    public XmlElement? Parent(Transaction transaction)
    {
        var handle = XmlElementChannel.Parent(Handle, transaction.Handle);

        return handle != nint.Zero ? new XmlElement(handle) : null;
    }

    /// <summary>
    ///     Returns an <see cref="XmlTreeWalker" /> for this <see cref="XmlElement" />.
    /// </summary>
    /// <remarks>
    ///     Check the documentation of <see cref="XmlTreeWalker" /> for more information.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>An <see cref="XmlTreeWalker" /> for this <see cref="XmlElement" />.</returns>
    public XmlTreeWalker TreeWalker(Transaction transaction)
    {
        var handle = XmlElementChannel.TreeWalker(Handle, transaction.Handle);

        return new XmlTreeWalker(handle.Checked());
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable Observe(Action<XmlElementEvent> action)
    {
        XmlElementChannel.ObserveCallback callback = (_, eventHandle) => action(new XmlElementEvent(eventHandle));

        var subscriptionId = XmlElementChannel.Observe(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            XmlElementChannel.Unobserve(Handle, subscriptionId);
        });
    }
}
