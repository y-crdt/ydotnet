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

    internal nint Handle { get; private set; }

    /// <summary>
    ///     Replaces the native handle of this resource.
    /// </summary>
    /// <remarks>
    ///     The caller is responsible for releasing the previous handle.
    /// </remarks>
    /// <param name="handle">The new native handle.</param>
    private protected void ReplaceHandle(nint handle)
    {
        Handle = handle;
    }
}
