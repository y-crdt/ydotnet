using YDotNet.Infrastructure;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

/// <summary>
///     Represents a logical identifier for a shared collection.
/// </summary>
public class BranchId
{
    internal BranchId(BranchIdNative native)
    {
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
}
