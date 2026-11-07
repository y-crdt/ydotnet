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
public abstract class NativeIterator : UnmanagedResource
{
    private readonly Branch branch;
    private readonly Transaction transaction;
    private readonly CreateHandleFunction createHandle;

    private protected NativeIterator(Branch branch, Transaction transaction, CreateHandleFunction createHandle)
        : base(CreateInitialHandle(branch, transaction, createHandle))
    {
        this.branch = branch;
        this.transaction = transaction;
        this.createHandle = createHandle;
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
        DestroyHandle(Handle);
        ReplaceHandle(newHandle);
    }

    /// <summary>
    ///     Releases the native iterator identified by <paramref name="handle" />.
    /// </summary>
    /// <param name="handle">The handle of the native iterator.</param>
    private protected abstract void DestroyHandle(nint handle);

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        DestroyHandle(Handle);
    }

    private static nint CreateInitialHandle(Branch branch, Transaction transaction, CreateHandleFunction createHandle)
    {
        ArgumentNullException.ThrowIfNull(branch);
        ArgumentNullException.ThrowIfNull(transaction);
        ArgumentNullException.ThrowIfNull(createHandle);

        return CreateHandle(branch, transaction, createHandle);
    }

    private static nint CreateHandle(Branch branch, Transaction transaction, CreateHandleFunction createHandle)
    {
        // Check the transaction first: reading the branch with a committed transaction crashes the native library.
        var transactionHandle = transaction.CheckedHandle();

        return createHandle(branch.GetHandle(transaction), transactionHandle).Checked();
    }
}
