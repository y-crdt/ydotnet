using YDotNet.Document.Cells;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents an array.
/// </summary>
public class Array : Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Array" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Array(nint handle)
        : base(handle)
    {
        // Nothing here.
    }

    /// <summary>
    ///     Gets the number of elements stored within current instance of <see cref="Types.Array" />.
    /// </summary>
    public uint Length => ArrayChannel.Length(Handle);

    /// <summary>
    ///     Inserts a range of <see cref="inputs" /> into the current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The starting index to insert the items.</param>
    /// <param name="inputs">The items to be inserted.</param>
    public void InsertRange(Transaction transaction, uint index, IEnumerable<Input> inputs)
    {
        var inputsArray = inputs.Select(x => x.InputNative).ToArray();
        var inputsPointer = MemoryWriter.WriteStructArray(inputsArray);

        ArrayChannel.InsertRange(Handle, transaction.Handle, index, inputsPointer, (uint) inputsArray.Length);
    }

    /// <summary>
    ///     Removes a range of items from the current instance of <see cref="Array" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="index">The starting index to remove the items.</param>
    /// <param name="length">The amount of items to remove.</param>
    public void RemoveRange(Transaction transaction, uint index, uint length)
    {
        ArrayChannel.RemoveRange(Handle, transaction.Handle, index, length);
    }
}
