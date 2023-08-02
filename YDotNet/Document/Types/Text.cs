using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents a text string.
/// </summary>
public class Text
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Text" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Text(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Inserts a string in the given `index`.
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
        MemoryWriter.TryToWriteStruct(attributes?.InputNative, out var attributesPointer);
        TextChannel.Insert(Handle, transaction.Handle, index, value, attributesPointer);
        MemoryWriter.TryRelease(attributesPointer);
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
        TextChannel.Format(Handle, transaction.Handle, index, length, attributesPointer);
        MemoryWriter.Release(attributesPointer);
    }

    /// <summary>
    ///     Returns the full string stored in the instance.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this read operation.</param>
    /// <returns>The full string stored in the instance.</returns>
    public string String(Transaction transaction)
    {
        // Get the string pointer and read it into a managed instance.
        var pointer = TextChannel.String(Handle, transaction.Handle);
        var result = Marshal.PtrToStringAnsi(pointer);

        // Dispose the resources used by the underlying string.
        StringChannel.Destroy(pointer);

        return result;
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
        return TextChannel.Length(Handle, transaction.Handle);
    }
}
