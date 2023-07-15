using System.Collections;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types;

/// <summary>
///     Represents the iterator to provide instances of <see cref="MapEntry" /> or <c>null</c> to
///     <see cref="MapIterator" />.
/// </summary>
internal class MapEnumerator : IEnumerator<MapEntry>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapEnumerator" /> class.
    /// </summary>
    /// <param name="mapIterator">
    ///     The <see cref="MapIterator" /> instance used by this enumerator.
    ///     Check <see cref="MapIterator" /> for more details.
    /// </param>
    internal MapEnumerator(MapIterator mapIterator)
    {
        MapIterator = mapIterator;
        Current = null;
    }

    /// <summary>
    ///     Gets the <see cref="MapIterator" /> instance that holds the
    ///     <see cref="Types.MapIterator.Handle" /> used by this enumerator.
    /// </summary>
    private MapIterator MapIterator { get; }

    /// <inheritdoc />
    object? IEnumerator.Current => Current;

    /// <inheritdoc />
    public MapEntry? Current { get; private set; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = MapChannel.IteratorNext(MapIterator.Handle);

        Current = handle != nint.Zero ? new MapEntry(handle) : null;

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
        MapIterator.Dispose();
    }
}
