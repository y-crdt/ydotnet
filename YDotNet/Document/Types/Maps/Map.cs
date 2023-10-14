using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     A shared data type that represents a map.
/// </summary>
public class Map : Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Map" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Map(nint handle)
        : base(handle)
    {
        // Nothing here.
    }

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
        var keyHandle = MemoryWriter.WriteUtf8String(key);
        var valueHandle = MemoryWriter.WriteStruct(input.InputNative);

        MapChannel.Insert(Handle, transaction.Handle, keyHandle, valueHandle);

        MemoryWriter.Release(keyHandle);
        MemoryWriter.Release(valueHandle);
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
        var keyHandle = MemoryWriter.WriteUtf8String(key);
        var handle = MapChannel.Get(Handle, transaction.Handle, keyHandle);

        MemoryWriter.Release(keyHandle);

        return ReferenceAccessor.Access(new Output(handle, disposable: true));
    }

    /// <summary>
    ///     Gets the number of entries stored in the <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The number of entries stored in the <see cref="Map" />.</returns>
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
        var keyHandle = MemoryWriter.WriteUtf8String(key);
        var result = MapChannel.Remove(Handle, transaction.Handle, keyHandle) == 1;

        MemoryWriter.Release(keyHandle);

        return result;
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

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="Map" /> instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription Observe(Action<MapEvent> action)
    {
        var subscriptionId = MapChannel.Observe(
            Handle,
            nint.Zero,
            (_, eventHandle) => action(new MapEvent(eventHandle)));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed within <see cref="Map" /> scope.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void Unobserve(EventSubscription subscription)
    {
        MapChannel.Unobserve(Handle, subscription.Id);
    }
}
