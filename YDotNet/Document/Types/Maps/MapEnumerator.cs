using System.Collections;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents the iterator to provide instances of <see cref="MapEntry" />.
/// </summary>
internal class MapEnumerator : IEnumerator<MapEntry>
{
    private MapEntry? current;

    internal MapEnumerator(MapIterator iterator)
    {
        Iterator = iterator;
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    /// <inheritdoc />
    public MapEntry Current => current!;

    /// <inheritdoc />
    object? IEnumerator.Current => current!;

    private MapIterator Iterator { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = MapChannel.IteratorNext(Iterator.Handle);

        // The map entry also manages the value of the output.
        current = handle != nint.Zero ? new MapEntry(handle, Iterator) : null;

        return current != null;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
