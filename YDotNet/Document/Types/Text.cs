using YDotNet.Document.Transactions;
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
    public void Insert(Transaction transaction, uint index, string value)
    {
        TextChannel.Insert(Handle, transaction.Handle, index, value, nint.Zero);
    }

    // TODO [LSViana] Add documentation
    public string String(Transaction transaction)
    {
        return TextChannel.String(Handle, transaction.Handle);
    }
}
