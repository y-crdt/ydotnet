using System.Runtime.InteropServices;

namespace YDotNet.Native.Document;

internal abstract class UnmanagedResourceHandle : SafeHandle
{
    protected UnmanagedResourceHandle()
        : base(IntPtr.Zero, ownsHandle: true)
    {
    }

    public override bool IsInvalid =>
        handle == IntPtr.Zero;
}
