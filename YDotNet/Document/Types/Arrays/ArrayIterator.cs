using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays;

/// <summary>
///     Represents an iterator, which can be used to traverse over all elements of an <see cref="Array" />.
/// </summary>
/// <remarks>
///     Check <see cref="NativeIterator" /> for how many times the iterator can be enumerated.
/// </remarks>
public class ArrayIterator : NativeIterator, IEnumerable<Output>
{
    internal ArrayIterator(Array array, Transaction transaction)
        : base(array, transaction, ArrayChannel.Iterator, ArrayChannel.IteratorDestroy)
    {
    }

    /// <inheritdoc />
    public IEnumerator<Output> GetEnumerator()
    {
        ThrowIfDisposed();
        return new ArrayEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
