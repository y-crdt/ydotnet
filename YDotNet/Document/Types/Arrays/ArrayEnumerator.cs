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

    internal ArrayEnumerator(ArrayIterator iterator)
    {
        Iterator = iterator;
    }

    /// <inheritdoc />
    public void Dispose()
    {
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

        // The output has now owner and can just be disposed when not needed anymore.
        current = handle != nint.Zero ? new Output(handle, null) : null;

        return current != null;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
