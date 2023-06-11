using YDotNet.Native.Transaction;

namespace YDotNet.Document.Transactions;

/// <summary>
///     Represents operations that are executed against a document. This can be reading or writing data.
/// </summary>
/// <remarks>
///     All operations that need to touch or modify the contents of a document need to be executed through a transaction.
/// </remarks>
public class Transaction
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Transaction" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Transaction(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Commit and dispose provided read-write transaction.
    /// </summary>
    /// <remarks>
    ///     This operation releases allocated resources, triggers update events and performs a storage compression over all
    ///     operations executed in scope of a current transaction.
    /// </remarks>
    public void Commit()
    {
        TransactionChannel.Commit(Handle);
    }
}
