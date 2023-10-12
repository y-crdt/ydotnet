using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.StickyIndexes;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.StickyIndex;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlTexts;

/// <summary>
///     A shared data type that represents a XML text.
/// </summary>
public class XmlText : Branch
{
    private readonly EventSubscriptions subscriptions = new();

    internal XmlText(nint handle, Doc doc)
        : base(handle, doc)
    {
    }

    /// <summary>
    ///     Gets the length of the text, in bytes, stored in this <see cref="XmlText" /> instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The length of the text, in bytes, stored in the <see cref="XmlText" />.</returns>
    public uint Length(Transaction transaction)
    {
        return XmlTextChannel.Length(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Inserts a string in the given <c>index</c>.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this write operation.</param>
    /// <param name="index">The index must be between 0 and <see cref="Length" /> or an exception will be thrown.</param>
    /// <param name="value">The text to be inserted.</param>
    /// <param name="attributes">
    ///     Optional, the attributes to be added to the inserted text. The value must be the result of a call to
    ///     <see cref="Input" />.<see cref="Input.Object" />.
    /// </param>
    public void Insert(Transaction transaction, uint index, string value, Input? attributes = null)
    {
        using var unsafeValue = MemoryWriter.WriteUtf8String(value);
        using var unsafeAttributes = MemoryWriter.WriteStruct(attributes?.InputNative);

        XmlTextChannel.Insert(Handle, transaction.Handle, index, unsafeValue.Handle, unsafeAttributes.Handle);
    }

    /// <summary>
    ///     Inserts a content in the given <c>index</c>.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this write operation.</param>
    /// <param name="index">The index must be between 0 and <see cref="Length" /> or an exception will be thrown.</param>
    /// <param name="content">The text to be inserted.</param>
    /// <param name="attributes">
    ///     Optional, the attributes to be added to the inserted text. The value must be the result of a call to
    ///     <see cref="Input" />.<see cref="Input.Object" />.
    /// </param>
    public void InsertEmbed(Transaction transaction, uint index, Input content, Input? attributes = null)
    {
        using var unsafeContent = MemoryWriter.WriteStruct(content.InputNative);
        using var unsafeAttributes = MemoryWriter.WriteStruct(attributes?.InputNative);

        XmlTextChannel.InsertEmbed(Handle, transaction.Handle, index, unsafeContent.Handle, unsafeAttributes.Handle);
    }

    /// <summary>
    ///     Inserts an attribute.
    /// </summary>
    /// <remarks>
    ///     If another attribute with the same <paramref name="name" /> already exists, it will be replaced.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be added.</param>
    /// <param name="value">The value of the attribute to be added.</param>
    public void InsertAttribute(Transaction transaction, string name, string value)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);
        using var unsafeValue = MemoryWriter.WriteUtf8String(value);

        XmlTextChannel.InsertAttribute(Handle, transaction.Handle, unsafeName.Handle, unsafeValue.Handle);
    }

    /// <summary>
    ///     Removes an attribute.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be removed.</param>
    public void RemoveAttribute(Transaction transaction, string name)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        XmlTextChannel.RemoveAttribute(Handle, transaction.Handle, unsafeName.Handle);
    }

    /// <summary>
    ///     Gets an attribute with the given <paramref name="name" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be retrieved.</param>
    /// <returns>The value of the attribute or <c>null</c> if it doesn't exist.</returns>
    public string? GetAttribute(Transaction transaction, string name)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = XmlTextChannel.GetAttribute(Handle, transaction.Handle, unsafeName.Handle);

        return handle != nint.Zero ? MemoryReader.ReadStringAndDestroy(handle) : null;
    }

    /// <summary>
    ///     Returns a <see cref="XmlAttributeIterator" />, which can be used to traverse over all attributes.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The <see cref="XmlAttributeIterator" /> instance.</returns>
    public XmlAttributeIterator Iterate(Transaction transaction)
    {
        var handle = XmlTextChannel.AttributeIterator(Handle, transaction.Handle).Checked();

        return new XmlAttributeIterator(handle);
    }

    /// <summary>
    ///     Returns the string representation of the <see cref="XmlText" /> instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The string representation of the <see cref="XmlText" /> instance.</returns>
    public string String(Transaction transaction)
    {
        var handle = XmlTextChannel.String(Handle, transaction.Handle);

        return MemoryReader.ReadStringAndDestroy(handle);
    }

    /// <summary>
    ///     Removes a range of characters.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this write operation.</param>
    /// <param name="index">The index must be between 0 and <see cref="Length" /> or an exception will be thrown.</param>
    /// <param name="length">
    ///     The length of the text to be removed, relative to the index and must not go over total
    ///     <see cref="Length" />.
    /// </param>
    public void RemoveRange(Transaction transaction, uint index, uint length)
    {
        XmlTextChannel.RemoveRange(Handle, transaction.Handle, index, length);
    }

    /// <summary>
    ///     Formats a string in the given range defined by <c>index</c> and <c>length</c>.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this write operation.</param>
    /// <param name="index">The index must be between 0 and <see cref="Length" /> or an exception will be thrown.</param>
    /// <param name="length">
    ///     The length of the text to be formatted, relative to the index and must not go over total
    ///     <see cref="Length" />.
    /// </param>
    /// <param name="attributes">
    ///     Optional, the attributes to be added to the inserted text. The value must be the result of a call to
    ///     <see cref="Input" />.<see cref="Input.Object" />.
    /// </param>
    public void Format(Transaction transaction, uint index, uint length, Input attributes)
    {
        using var unsafeAttributes = MemoryWriter.WriteStruct(attributes?.InputNative);

        XmlTextChannel.Format(Handle, transaction.Handle, index, length, unsafeAttributes.Handle);
    }

    /// <summary>
    ///     Returns the previous sibling which can be either an <see cref="XmlElement" />
    ///     or an <see cref="XmlText" /> or <c>null</c> if this node is the first child.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The previous sibling of which can be either an <see cref="XmlElement" /> or
    ///     an <see cref="XmlText" /> or <c>null</c> if this node is the first child.
    /// </returns>
    public Output? PreviousSibling(Transaction transaction)
    {
        var handle = XmlChannel.PreviousSibling(Handle, transaction.Handle);

        return handle != nint.Zero ? new Output(handle, Doc, null) : null;
    }

    /// <summary>
    ///     Returns the next sibling which can be either an <see cref="XmlElement" />
    ///     or an <see cref="XmlText" /> or <c>null</c> if this node is the last child.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The next sibling which can be either an <see cref="XmlElement" /> or an
    ///     <see cref="XmlText" /> or <c>null</c> if this node is the last child.
    /// </returns>
    public Output? NextSibling(Transaction transaction)
    {
        var handle = XmlChannel.NextSibling(Handle, transaction.Handle);

        return handle != nint.Zero ? new Output(handle, Doc, null) : null;
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed in this instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable Observe(Action<XmlTextEvent> action)
    {
        XmlTextChannel.ObserveCallback callback = (_, eventHandle) => action(new XmlTextEvent(eventHandle, Doc));

        var subscriptionId = XmlTextChannel.Observe(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            XmlTextChannel.Unobserve(Handle, subscriptionId);
        });
    }

    /// <summary>
    ///     Retrieves a <see cref="StickyIndex" /> corresponding to a given human-readable <paramref name="index" /> pointing into
    ///     the <see cref="Branch" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The numeric index to place the <see cref="StickyIndex" />.</param>
    /// <param name="associationType">The type of the <see cref="StickyIndex" />.</param>
    /// <returns>The <see cref="StickyIndex" /> in the <paramref name="index" /> with the given <paramref name="associationType" />.</returns>
    public StickyIndex? StickyIndex(Transaction transaction, uint index, StickyAssociationType associationType)
    {
        var handle = StickyIndexChannel.FromIndex(Handle, transaction.Handle, index, (sbyte)associationType);

        return handle != nint.Zero ? new StickyIndex(handle) : null;
    }
}
