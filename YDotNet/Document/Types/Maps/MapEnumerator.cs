using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents the iterator to provide instances of key-value pairs.
/// </summary>
internal class MapEnumerator : IEnumerator<KeyValuePair<string, Output>>
{
    private readonly MapIterator iterator;

    internal MapEnumerator(MapIterator iterator)
    {
        this.iterator = iterator;
    }

    /// <inheritdoc />
    public KeyValuePair<string, Output> Current { get; private set; }

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public void Dispose()
    {
        iterator.Dispose();
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = MapChannel.IteratorNext(iterator.Handle);

        if (handle != nint.Zero)
        {
            var native = MemoryReader.ReadStruct<MapEntryNative>(handle);

            Current = new KeyValuePair<string, Output>(
                native.Key(),
                new Output(native.ValueHandle(handle), iterator.Doc, isDeleted: false));

            // We are done reading and can destroy the resource.
            MapChannel.EntryDestroy(handle);

            return true;
        }

        Current = default;
        return false;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotSupportedException();
    }
}
