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

    private ArrayIterator Iterator { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = ArrayChannel.IteratorNext(Iterator.Handle);

        if (handle != nint.Zero)
        {
            current = Output.CreateAndRelease(handle, Iterator.Doc);
            return true;
        }
        else
        {
            current = null!;
            return false;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
