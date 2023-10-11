namespace YDotNet.Infrastructure;

public abstract class UnmanagedResource : Resource
{
    protected UnmanagedResource(nint handle, IResourceOwner? owner = null)
        : base(owner)
    {
        Handle = handle;
    }

    public nint Handle { get; }
}
