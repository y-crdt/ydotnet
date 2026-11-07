using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents an enumerable to read key-value pairs from a <see cref="Map" />.
/// </summary>
/// <remarks>
///     Two important details about <see cref="MapIterator" />.
///     <ul>
///         <li>The entries instances are unordered when iterating;</li>
///         <li>Check <see cref="NativeIterator" /> for how many times the iterator can be enumerated.</li>
///     </ul>
/// </remarks>
public class MapIterator : NativeIterator, IEnumerable<KeyValuePair<string, Output>>
{
    internal MapIterator(Map map, Transaction transaction, Doc doc)
        : base(map, transaction, MapChannel.Iterator)
    {
        Doc = doc;
    }

    internal Doc Doc { get; }

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, Output>> GetEnumerator()
    {
        ThrowIfDisposed();
        return new MapEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <inheritdoc />
    private protected override void DestroyHandle(nint handle)
    {
        MapChannel.IteratorDestroy(handle);
    }
}
