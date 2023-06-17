using System.Runtime.InteropServices;
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

    /// <summary>
    ///     Returns the sub-documents existing within current document.
    /// </summary>
    /// <returns>The sub-documents existing within current document.</returns>
    public Doc[] SubDocs()
    {
        var handle = TransactionChannel.SubDocs(Handle, out var length);
        var docs = new Doc[length];

        for (var i = 0; i < length; i++)
        {
            var doc = new Doc(Marshal.ReadIntPtr(handle, i * nint.Size));
            docs[i] = doc;
        }

        return docs;
    }
}
