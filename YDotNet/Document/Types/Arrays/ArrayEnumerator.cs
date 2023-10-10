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
    private Output? current;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ArrayEnumerator" /> class.
    /// </summary>
    /// <param name="iterator">
    ///     The <see cref="Iterator" /> instance used by this enumerator.
    ///     Check <see cref="Iterator" /> for more details.
    /// </param>
    internal ArrayEnumerator(ArrayIterator iterator)
    {
        Iterator = iterator;
    }

    /// <inheritdoc />
    public Output Current => current!;

    /// <inheritdoc />
    object? IEnumerator.Current => current!;

    /// <summary>
    ///     Gets the <see cref="Iterator" /> instance that holds the <see cref="ArrayIterator.Handle" /> used by this enumerator.
    /// </summary>
    private ArrayIterator Iterator { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = ArrayChannel.IteratorNext(Iterator.Handle);

        current = handle != nint.Zero ? new Output(handle, false) : null;

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
        Iterator.Dispose();
    }
}
