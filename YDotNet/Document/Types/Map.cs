using YDotNet.Document.Transactions;
using YDotNet.Native.Cells;
using YDotNet.Native.Types;

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
    ///     If entry under such given key already existed, then the entry will be replaced.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this write operation.</param>
    /// <param name="key">The key to be used to identify this entry.</param>
    /// <param name="doc">The <see cref="Doc" /> instance to be inserted as a sub-document.</param>
    public void Insert(Transaction transaction, string key, Doc doc)
    {
        Insert(transaction, key, InputChannel.Doc(doc.Handle));
    }

    /// <summary>
    ///     Remove an entry, specified as a key, from the current <see cref="Map" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this write operation.</param>
    /// <param name="key">The key to be used to identify this entry.</param>
    /// <returns>`true` if the entry was found and removed, `false` if no entry was found.</returns>
    public bool Remove(Transaction transaction, string key)
    {
        return MapChannel.Remove(Handle, transaction.Handle, key);
    }

    private void Insert(Transaction transaction, string key, Input input)
    {
        MapChannel.Insert(Handle, transaction.Handle, key, input);
    }
}
