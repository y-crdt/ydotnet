using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.StickyIndexes;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.Types.Texts.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.StickyIndex;
using YDotNet.Native.Types.Texts;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     A shared data type that represents a text string.
/// </summary>
public class Text : Branch
{
    private readonly EventSubscriber<TextEvent> onObserve;

    internal Text(nint handle, Doc doc, bool isDeleted)
        : base(handle, doc, isDeleted)
    {
        onObserve = new EventSubscriber<TextEvent>(
            handle,
            (text, action) =>
            {
                TextChannel.ObserveCallback callback = (_, eventHandle) =>
                    action(new TextEvent(eventHandle, Doc));

                return (TextChannel.Observe(text, nint.Zero, callback), callback);
            },
            TextChannel.Unobserve);
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
        ThrowIfDisposed();

        using var unsafeValue = MemoryWriter.WriteUtf8String(value);
        using var unsafeAttributes = MemoryWriter.WriteStruct(attributes?.InputNative);

        TextChannel.Insert(Handle, transaction.Handle, index, unsafeValue.Handle, unsafeAttributes.Handle);
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
        ThrowIfDisposed();

        var unsafeContent = MemoryWriter.WriteStruct(content.InputNative);
        var unsafeAttributes = MemoryWriter.WriteStruct(attributes?.InputNative);

        TextChannel.InsertEmbed(Handle, transaction.Handle, index, unsafeContent.Handle, unsafeAttributes.Handle);
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
        ThrowIfDisposed();

        TextChannel.RemoveRange(Handle, transaction.Handle, index, length);
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
        ThrowIfDisposed();

        using var unsafeAttributes = MemoryWriter.WriteStruct(attributes.InputNative);

        TextChannel.Format(Handle, transaction.Handle, index, length, unsafeAttributes.Handle);
    }

    /// <summary>
    ///     Returns the <see cref="TextChunks" /> that compose this <see cref="Text" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this read operation.</param>
    /// <returns>The <see cref="TextChunks" /> that compose this <see cref="Text" />.</returns>
    public TextChunks Chunks(Transaction transaction)
    {
        ThrowIfDisposed();

        var handle = TextChannel.Chunks(Handle, transaction.Handle, out var length).Checked();

        return new TextChunks(handle, length, Doc);
    }

    /// <summary>
    ///     Returns the full string stored in the instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this read operation.</param>
    /// <returns>The full string stored in the instance.</returns>
    public string String(Transaction transaction)
    {
        ThrowIfDisposed();

        var handle = TextChannel.String(Handle, transaction.Handle);

        return MemoryReader.ReadStringAndDestroy(handle);
    }

    /// <summary>
    ///     Returns the length of the string, in bytes, stored in the instance.
    /// </summary>
    /// <remarks>
    ///     The returned value doesn't include the null terminator.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this read operation.</param>
    /// <returns>The length, in bytes, of the string stored in the instance.</returns>
    public uint Length(Transaction transaction)
    {
        ThrowIfDisposed();

        return TextChannel.Length(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Text" /> scope.
    /// </summary>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable Observe(Action<TextEvent> action)
    {
        ThrowIfDisposed();

        return onObserve.Subscribe(action);
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
        ThrowIfDisposed();

        var handle = StickyIndexChannel.FromIndex(Handle, transaction.Handle, index, (sbyte)associationType);

        return handle != nint.Zero ? new StickyIndex(handle) : null;
    }
}
