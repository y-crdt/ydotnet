using YDotNet.Document.Transactions;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     Represents a logical identifier for a shared collection.
/// </summary>
internal record struct BranchId
{
    private readonly bool isDeleted;

    internal BranchId(BranchIdNative native, bool isDeleted)
    {
        Native = native;

        this.isDeleted = isDeleted;
    }

    public static BranchId FromHandle(nint handle, bool isDeleted)
    {
        return new BranchId(isDeleted ? default : BranchChannel.Id(handle), isDeleted);
    }

    private BranchIdNative Native { get; }

    /// <summary>
    ///     <para>Gets a value indicating whether <see cref="ClientId" /> and <see cref="Clock" /> have values.</para>
    ///     <para>If this is <c>false</c>, check <see cref="HasName" />.</para>
    /// </summary>
    public bool HasClientIdAndClock => Native.ClientIdOrLength > 0;

    /// <summary>
    ///     Gets a value indicating whether <see cref="Name" /> has values.
    ///     <para>If this is <c>false</c>, check <see cref="HasClientIdAndClock" />.</para>
    /// </summary>
    public bool HasName => !HasClientIdAndClock;

    /// <summary>
    ///     Gets the client ID of a creator of a nested shared type that this <see cref="BranchId" /> points to.
    /// </summary>
    public long? ClientId => HasClientIdAndClock ? Native.ClientIdOrLength : null;

    /// <summary>
    ///     Gets the clock number timestamp when the creator of a nested shared type created it.
    /// </summary>
    public uint? Clock => HasClientIdAndClock ? Native.BranchIdVariant.Clock : null;

    /// <summary>
    ///     Gets the name of the root-level shared type.
    /// </summary>
    public string? Name => HasClientIdAndClock ? null : MemoryReader.ReadUtf8String(Native.BranchIdVariant.NamePointer);

    public nint GetHandle(Transaction transaction)
    {
        if (isDeleted)
        {
            throw new ObjectDisposedException("Object is disposed.");
        }

        var handle = MemoryWriter.WriteStruct(Native);

        var branchHandle = BranchChannel.Get(handle.Handle, transaction.Handle);

        handle.Dispose();

        if (branchHandle == nint.Zero || BranchChannel.Alive(branchHandle) == 0)
        {
            throw new ObjectDisposedException("Object is disposed.");
        }

        return branchHandle;
    }
}
