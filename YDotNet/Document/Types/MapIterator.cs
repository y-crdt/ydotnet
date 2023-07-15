using System.Collections;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types;

/// <summary>
///     Represents an enumerable to read <see cref="MapEntry" /> instances from a <see cref="Map" />.
/// </summary>
/// <remarks>
///     Two important details about <see cref="MapIterator" />:
///     <ul>
///         <li>The <see cref="MapEntry" /> instances are unordered when iterating;</li>
///         <li>
///             The iterator can't be reused. If needed, use <see cref="Enumerable.ToArray{TSource}" /> to accumulate
///             values.
///         </li>
///     </ul>
/// </remarks>
public class MapIterator : IEnumerable<MapEntry>, IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapIterator" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal MapIterator(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        MapChannel.IteratorDestroy(Handle);
    }

    /// <inheritdoc />
    public IEnumerator<MapEntry> GetEnumerator()
    {
        return new MapEnumerator(this);
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
