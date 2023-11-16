namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for all unmanaged resources.
/// </summary>
public abstract class UnmanagedResource : Resource
{
    internal protected UnmanagedResource(nint handle, bool isDisposed = false)
        : base(isDisposed)
    {
        Handle = handle;
    }

    internal nint Handle { get; }
}
