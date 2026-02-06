namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for all unmanaged resources.
/// </summary>
public abstract class UnmanagedResource : Resource
{
    protected internal UnmanagedResource(nint handle, bool isDisposed = false)
        : base(isDisposed)
    {
        Handle = handle;
    }

    internal nint Handle { get; }

    /// <summary>
    ///     Checks whether this managed object still corresponds to the native resource at the given handle.
    ///     Returns <c>false</c> when the native allocator has reused the address for a different resource
    ///     (e.g. after CRDT garbage collection freed the original).
    /// </summary>
    /// <param name="handle">The native handle to validate against.</param>
    /// <returns><c>true</c> if this object still represents the resource at <paramref name="handle"/>.</returns>
    internal virtual bool MatchesHandle(nint handle)
    {
        return true;
    }
}
