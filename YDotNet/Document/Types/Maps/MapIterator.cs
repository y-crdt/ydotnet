using System.Collections;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents an enumerable to read <see cref="MapEntry" /> instances from a <see cref="Map" />.
/// </summary>
/// <remarks>
///     Two important details about <see cref="MapIterator" />.
///     <ul>
///         <li>The <see cref="MapEntry" /> instances are unordered when iterating;</li>
///         <li>
///             The iterator can't be reused. If needed, use <see cref="Enumerable.ToArray{TSource}" /> to accumulate
///             values.
///         </li>
///     </ul>
/// </remarks>
public class MapIterator : UnmanagedResource, IEnumerable<MapEntry>
{
    internal MapIterator(nint handle)
        : base(handle)
    {
    }

    ~MapIterator()
    {
        Dispose(false);
    }

    protected override void DisposeCore(bool disposing)
    {
        MapChannel.IteratorDestroy(Handle);
    }

    /// <inheritdoc />
    public IEnumerator<MapEntry> GetEnumerator()
    {
        ThrowIfDisposed();
        return new MapEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
