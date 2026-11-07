using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure.Extensions;

namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for iterators that read the contents of a <see cref="Branch" /> through a native iterator.
/// </summary>
/// <remarks>
///     The iterator is disposed when its enumerator is disposed, so it can be enumerated only once (for example,
///     with a single <c>foreach</c>). Within that enumeration, <see cref="System.Collections.IEnumerator.Reset" />
///     restarts it from the beginning. To read the values more than once, use
///     <see cref="Enumerable.ToArray{TSource}" /> to accumulate them.
/// </remarks>
public class NativeIterator : UnmanagedResource
{
    private readonly Branch branch;
    private readonly Transaction transaction;
    private readonly CreateHandleFunction createHandle;
    private readonly DestroyHandleFunction destroyHandle;

    private protected NativeIterator(
        Branch branch,
        Transaction transaction,
        CreateHandleFunction createHandle,
        DestroyHandleFunction destroyHandle)
        : base(CreateInitialHandle(branch, transaction, createHandle, destroyHandle))
    {
        this.branch = branch;
        this.transaction = transaction;
        this.createHandle = createHandle;
        this.destroyHandle = destroyHandle;
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="NativeIterator" /> class.
    /// </summary>
    ~NativeIterator()
    {
        Dispose(disposing: false);
    }

    /// <summary>
    ///     Creates a native iterator over the contents of a branch.
    /// </summary>
    /// <param name="branchHandle">The native handle of the branch to iterate.</param>
    /// <param name="transactionHandle">The native handle of the transaction that wraps the iteration.</param>
    /// <returns>The native handle of the new iterator, or <c>0</c> if it couldn't be created.</returns>
    internal delegate nint CreateHandleFunction(nint branchHandle, nint transactionHandle);

    /// <summary>
    ///     Releases a native iterator.
    /// </summary>
    /// <param name="handle">The native handle of the iterator to release.</param>
    internal delegate void DestroyHandleFunction(nint handle);

    /// <summary>
    ///     Gets the document that owns the iterated branch.
    /// </summary>
    internal Doc Doc => branch.Doc;

    /// <summary>
    ///     Recreates the native iterator so the enumeration starts again from the first item.
    /// </summary>
    /// <exception cref="ObjectDisposedException">
    ///     The iterator or the <see cref="Transaction" /> used to create it has been disposed.
    /// </exception>
    internal void Reset()
    {
        ThrowIfDisposed();

        var newHandle = CreateHandle(branch, transaction, createHandle);
        destroyHandle(Handle);
        ReplaceHandle(newHandle);
    }

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        // The finalizer also runs when the constructor threw, before the handle and the fields were set.
        if (Handle == nint.Zero)
        {
            return;
        }

        destroyHandle(Handle);
    }

    private static nint CreateInitialHandle(
        Branch branch,
        Transaction transaction,
        CreateHandleFunction createHandle,
        DestroyHandleFunction destroyHandle)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(createHandle);
        ArgumentNullException.ThrowIfNull(destroyHandle);

        return CreateHandle(branch, transaction, createHandle);
    }

    private static nint CreateHandle(Branch branch, Transaction transaction, CreateHandleFunction createHandle)
    {
        // Check the transaction first: reading the branch with a committed transaction crashes the native library.
        var transactionHandle = transaction.CheckedHandle();

        return createHandle(branch.GetHandle(transaction), transactionHandle).Checked();
    }
}
