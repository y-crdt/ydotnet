using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.Arrays;

/// <summary>
///     Represents the iterator to provide instances of <see cref="Output" /> or <c>null</c> to
///     <see cref="ArrayEnumerator" />.
/// </summary>
internal class ArrayEnumerator : IEnumerator<Output>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ArrayEnumerator" /> class.
    /// </summary>
    /// <param name="arrayIterator">
    ///     The <see cref="ArrayIterator" /> instance used by this enumerator.
    ///     Check <see cref="ArrayIterator" /> for more details.
    /// </param>
    internal ArrayEnumerator(ArrayIterator arrayIterator)
    {
        ArrayIterator = arrayIterator;
        Current = null;
    }

    /// <summary>
    ///     Gets the <see cref="ArrayIterator" /> instance that holds the
    ///     <see cref="Types.ArrayIterator.Handle" /> used by this enumerator.
    /// </summary>
    private ArrayIterator ArrayIterator { get; }

    /// <inheritdoc />
    object? IEnumerator.Current => Current;

    /// <inheritdoc />
    public Output? Current { get; private set; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = ArrayChannel.IteratorNext(ArrayIterator.Handle);

        Current = handle != nint.Zero ? new Output(handle) : null;

        return Current != null;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        ArrayIterator.Dispose();
    }
}
