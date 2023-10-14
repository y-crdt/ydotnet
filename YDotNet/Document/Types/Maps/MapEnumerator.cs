using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents the iterator to provide instances of key value pairs.
/// </summary>
internal class MapEnumerator : IEnumerator<KeyValuePair<string, Output>>
{
    private KeyValuePair<string, Output> current;

    internal MapEnumerator(MapIterator iterator)
    {
        Iterator = iterator;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Iterator.Dispose();
    }

    /// <inheritdoc />
    public KeyValuePair<string, Output> Current => current;

    /// <inheritdoc />
    object? IEnumerator.Current => current!;

    private MapIterator Iterator { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = MapChannel.IteratorNext(Iterator.Handle);

        if (handle != nint.Zero)
        {
            var native = MemoryReader.ReadStruct<MapEntryNative>(handle);

            current = new KeyValuePair<string, Output>(
                native.Key(),
                new Output(native.ValueHandle(handle), Iterator.Doc, false));

            // We are done reading, so we can release the memory.
            MapChannel.EntryDestroy(handle);

            return true;
        }
        else
        {
            current = default;
            return false;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
