using YDotNet.Infrastructure;
using YDotNet.Native.Types.Branches;

namespace YDotNet.Document.Types.Branches;

public class BranchId
{
    internal BranchId(BranchIdNative native)
    {
        Native = native;
    }

    private BranchIdNative Native { get; }

    public bool HasClientIdAndClock => Native.ClientIdOrLength > 0;

    public bool HasLengthAndName => !HasClientIdAndClock;

    public long? ClientId => HasClientIdAndClock ? Native.ClientIdOrLength : null;

    public uint? Clock => HasClientIdAndClock ? Native.BranchIdVariant.Clock : null;

    public long? Length => HasClientIdAndClock ? null : -Native.ClientIdOrLength;

    public string? Name => HasClientIdAndClock ? null : MemoryReader.ReadUtf8String(Native.BranchIdVariant.NamePointer);
}
