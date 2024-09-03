using YDotNet.Document.Cells;
using YDotNet.Document.Events;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.Types.Maps.Events;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     A shared data type that represents a map.
/// </summary>
public class Map : Branch
{
    private readonly EventSubscriberFromId<MapEvent> onObserve;

    internal Map(nint handle, Doc doc, bool isDeleted)
        : base(handle, doc, isDeleted)
    {
        onObserve = new EventSubscriberFromId<MapEvent>(
            doc.EventManager,
            this,
            (map, action) =>
            {
                MapChannel.ObserveCallback callback = (_, eventHandle) =>
                    action(new MapEvent(eventHandle, Doc));

                return (MapChannel.Observe(map, nint.Zero, callback), callback);
            },
            SubscriptionChannel.Unobserve);
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
        using var unsafeKey = MemoryWriter.WriteUtf8String(key);
        using var unsafeValue = MemoryWriter.WriteStruct(input.InputNative);

        MapChannel.Insert(GetHandle(transaction), transaction.Handle, unsafeKey.Handle, unsafeValue.Handle);
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
        using var unsafeName = MemoryWriter.WriteUtf8String(key);

        var handle = MapChannel.Get(GetHandle(transaction), transaction.Handle, unsafeName.Handle);

        return handle != nint.Zero ? Output.CreateAndRelease(handle, Doc) : null;
    }

    /// <summary>
    ///     Gets the number of entries stored in the <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The number of entries stored in the <see cref="Map" />.</returns>
    public uint Length(Transaction transaction)
    {
        return MapChannel.Length(GetHandle(transaction), transaction.Handle);
    }

    /// <summary>
    ///     Removes an entry, specified as a key, from the current <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="key">The key to be used to identify this entry.</param>
    /// <returns>`true` if the entry was found and removed, `false` if no entry was found.</returns>
    public bool Remove(Transaction transaction, string key)
    {
        using var unsafeKey = MemoryWriter.WriteUtf8String(key);

        return MapChannel.Remove(GetHandle(transaction), transaction.Handle, unsafeKey.Handle) == 1;
    }

    /// <summary>
    ///     Removes all entries from the current <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    public void RemoveAll(Transaction transaction)
    {
        MapChannel.RemoveAll(GetHandle(transaction), transaction.Handle);
    }

    /// <summary>
    ///     Returns a <see cref="MapIterator" />, which can be used to traverse over all key-value pairs of a
    ///     <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>The <see cref="MapIterator" /> instance.</returns>
    public MapIterator Iterate(Transaction transaction)
    {
        var handle = MapChannel.Iterator(GetHandle(transaction), transaction.Handle).Checked();

        return new MapIterator(handle, Doc);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within the <see cref="Map" /> instance.
    /// </summary>
    /// <remarks>
    ///     The callbacks are triggered whenever a <see cref="Transaction" /> is committed.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable Observe(Action<MapEvent> action)
    {
        return onObserve.Subscribe(action);
    }
}
