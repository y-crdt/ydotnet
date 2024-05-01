namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for all unmanaged resources.
/// </summary>
public abstract class UnmanagedResource : Resource
{
    protected internal UnmanagedResource(nint handle, bool isDisposed = false)
        : base(isDisposed)
    {
        this.Handle = handle;
    }

    internal nint Handle { get; }
}
