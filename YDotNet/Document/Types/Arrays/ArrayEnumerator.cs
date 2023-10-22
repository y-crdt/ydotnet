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
    private readonly ArrayIterator iterator;
    private Output? current;

    internal ArrayEnumerator(ArrayIterator iterator)
    {
        this.iterator = iterator;
    }

    /// <inheritdoc />
    public Output Current => current!;

    /// <inheritdoc />
    object IEnumerator.Current => current!;

    /// <inheritdoc />
    public void Dispose()
    {
        iterator.Dispose();
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = ArrayChannel.IteratorNext(iterator.Handle);

        if (handle != nint.Zero)
        {
            current = Output.CreateAndRelease(handle, iterator.Doc);
            return true;
        }

        current = null!;

        return false;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
