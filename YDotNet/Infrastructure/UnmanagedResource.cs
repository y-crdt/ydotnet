namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for all unmanaged resources.
/// </summary>
public abstract class UnmanagedResource : Resource
{
    internal UnmanagedResource(nint handle, IResourceOwner? owner = null)
        : base(owner)
    {
        Handle = handle;
    }

    internal nint Handle { get; }
}
