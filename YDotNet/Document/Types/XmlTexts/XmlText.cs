using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
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
}
