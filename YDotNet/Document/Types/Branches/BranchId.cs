using YDotNet.Document.Transactions;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     Represents a logical identifier for a shared collection.
/// </summary>
public class BranchId
{
    private readonly Doc doc;

    internal BranchId(BranchIdNative native, Doc doc)
    {
        this.doc = doc;

        Native = native;
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

    public Branch? Get(Transaction transaction)
    {
        var handle = MemoryWriter.WriteStruct(Native);

        var branchHandle = BranchChannel.Get(handle.Handle, transaction.Handle);

        handle.Dispose();

        if (branchHandle == nint.Zero)
        {
            return null;
        }

        var branchKind = (BranchKind) BranchChannel.Kind(branchHandle);
        var branchDeleted = BranchChannel.Alive(branchHandle) == 0;

        switch (branchKind)
        {
            case BranchKind.Array:
                return doc.GetArray(branchHandle, branchDeleted);
            case BranchKind.Map:
                return doc.GetMap(branchHandle, branchDeleted);
            case BranchKind.Text:
                return doc.GetText(branchHandle, branchDeleted);
            case BranchKind.XmlElement:
                return doc.GetXmlElement(branchHandle, branchDeleted);
            case BranchKind.XmlText:
                return doc.GetXmlText(branchHandle, branchDeleted);
            case BranchKind.XmlFragment:
                return doc.GetXmlFragment(branchHandle, branchDeleted);
            default:
                return null;
        }
    }
}
