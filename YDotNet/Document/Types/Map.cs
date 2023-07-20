using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents a map.
/// </summary>
public class Map
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Map" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Map(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Inserts a new entry (specified as key-value pair) into the current <see cref="Map" />.
    /// </summary>
    /// <remarks>
    ///     If the entry under key already existed, then the entry will be replaced.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="key">The key to be used to identify this entry.</param>
    /// <param name="input">The <see cref="Input" /> instance to be inserted.</param>
    public void Insert(Transaction transaction, string key, Input input)
    {
        MapChannel.Insert(Handle, transaction.Handle, key, input.InputNative);
    }

    /// <summary>
    ///     Gets an entry, based on the key, from the <see cref="Map" />.
    /// </summary>
    /// <remarks>
    ///     If the entry key does not exist, returns <c>null</c>.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="key">The key to be used to identify this entry.</param>
    /// <returns>The <see cref="Output" /> or <c>null</c> if entry not found.</returns>
    public Output? Get(Transaction transaction, string key)
    {
        var handle = MapChannel.Get(Handle, transaction.Handle, key);

        return ReferenceAccessor.Access(new Output(handle));
    }

    /// <summary>
    ///     Gets the number of entries stored in the <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The number of entries stored in the <see cref="Map" />.</returns>
    // TODO [LSViana] Check if the `Transaction` parameter is really needed here.
    public uint Length(Transaction transaction)
    {
        return MapChannel.Length(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Removes an entry, specified as a key, from the current <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="key">The key to be used to identify this entry.</param>
    /// <returns>`true` if the entry was found and removed, `false` if no entry was found.</returns>
    public bool Remove(Transaction transaction, string key)
    {
        return MapChannel.Remove(Handle, transaction.Handle, key) == 1;
    }

    /// <summary>
    ///     Removes all entries from the current <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    public void RemoveAll(Transaction transaction)
    {
        MapChannel.RemoveAll(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Returns a <see cref="MapIterator" />, which can be used to traverse over all key-value pairs of a
    ///     <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The <see cref="MapIterator" /> instance or <c>null</c> if failed.</returns>
    public MapIterator? Iterate(Transaction transaction)
    {
        return ReferenceAccessor.Access(new MapIterator(MapChannel.Iterator(Handle, transaction.Handle)));
    }
}
