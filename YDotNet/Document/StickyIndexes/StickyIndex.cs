using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.StickyIndex;
using YDotNet.Native.Types;

namespace YDotNet.Document.StickyIndexes;

/// <summary>
///     A sticky index keeps track of the number index of positions inside <see cref="Branch" /> instances
///     even when changes happen to the content.
/// </summary>
/// <remarks>
///     For example, placing sticky index before a certain character will always point to this character.
///     Also, placing a sticky index at the end of a <see cref="Branch" /> will always point to the end of that
///     <see cref="Branch" />.
/// </remarks>
public class StickyIndex : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StickyIndex" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal StickyIndex(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the <see cref="StickyAssociationType" /> of the current <see cref="StickyIndex" />.
    /// </summary>
    public StickyAssociationType AssociationType => (StickyAssociationType)StickyIndexChannel.AssociationType(Handle);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        StickyIndexChannel.Destroy(Handle);
    }

    /// <summary>
    ///     Creates a <see cref="StickyIndex" /> from the result of <see cref="Encode" />.
    /// </summary>
    /// <param name="encoded">The <see cref="byte" /> array received from <see cref="Encode" />.</param>
    /// <returns>The <see cref="StickyIndex" /> represented by the provided <see cref="byte" /> array.</returns>
    public static StickyIndex Decode(byte[] encoded)
    {
        var handle = StickyIndexChannel.Decode(encoded, (uint)encoded.Length);

        return new StickyIndex(handle.Checked());
    }

    /// <summary>
    ///     Returns the numeric index in the context of the related <see cref="Branch" /> that created this
    ///     <see cref="StickyIndex" />.
    /// </summary>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <returns>
    ///     The numeric index in the context of the related <see cref="Branch" /> that created this <see cref="StickyIndex" />.
    /// </returns>
    public uint Read(Transaction transaction)
    {
        StickyIndexChannel.Read(Handle, transaction.Handle, out _, out var index);

        return index;
    }

    /// <summary>
    ///     Serializes the <see cref="StickyIndex" /> into binary representation.
    /// </summary>
    /// <returns>The binary representation of the <see cref="StickyIndex" /> index.</returns>
    public byte[] Encode()
    {
        var handle = StickyIndexChannel.Encode(Handle, out var length);
        var result = MemoryReader.ReadBytes(handle, length);

        BinaryChannel.Destroy(handle, length);

        return result;
    }
}
