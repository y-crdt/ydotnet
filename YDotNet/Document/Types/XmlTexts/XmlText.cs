using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlTexts;

/// <summary>
///     A shared data type that represents a XML text.
/// </summary>
public class XmlText : Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlText" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlText(nint handle)
        : base(handle)
    {
        // Nothing here.
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
        var valueHandle = MemoryWriter.WriteUtf8String(value);
        MemoryWriter.TryToWriteStruct(attributes?.InputNative, out var attributesHandle);

        XmlTextChannel.Insert(Handle, transaction.Handle, index, valueHandle, attributesHandle);

        MemoryWriter.TryRelease(attributesHandle);
        MemoryWriter.Release(valueHandle);
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
        MemoryWriter.TryToWriteStruct(attributes?.InputNative, out var attributesPointer);
        MemoryWriter.TryToWriteStruct(content.InputNative, out var contentPointer);
        XmlTextChannel.InsertEmbed(Handle, transaction.Handle, index, contentPointer, attributesPointer);
        MemoryWriter.TryRelease(attributesPointer);
        MemoryWriter.TryRelease(contentPointer);
    }

    /// <summary>
    ///     Returns the string representation of the <see cref="XmlText" /> instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The string representation of the <see cref="XmlText" /> instance.</returns>
    public string String(Transaction transaction)
    {
        var handle = XmlTextChannel.String(Handle, transaction.Handle);
        var result = MemoryReader.ReadUtf8String(handle);
        StringChannel.Destroy(handle);

        return result;
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
        var attributesPointer = MemoryWriter.WriteStruct(attributes.InputNative);
        XmlTextChannel.Format(Handle, transaction.Handle, index, length, attributesPointer);
        MemoryWriter.Release(attributesPointer);
    }
}
