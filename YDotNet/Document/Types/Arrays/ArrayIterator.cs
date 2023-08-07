using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays;

/// <summary>
///     Represents an iterator, which can be used to traverse over all elements of an <see cref="Array" />.
/// </summary>
/// <remarks>
///     The iterator can't be reused. If needed, use <see cref="Enumerable.ToArray{TSource}" /> to accumulate values.
/// </remarks>
public class ArrayIterator : IEnumerable<Output>, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ArrayIterator" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal ArrayIterator(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        ArrayChannel.IteratorDestroy(Handle);
    }

    /// <inheritdoc />
    public IEnumerator<Output> GetEnumerator()
    {
        return new ArrayEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
