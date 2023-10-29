namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for all managed types.
/// </summary>
public abstract class TypeBase : UnmanagedResource
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TypeBase" /> class.
    /// </summary>
    /// <param name="handle">A handle to the unmanaged resource.</param>
    /// <param name="isDisposed">A value indicating if the instance is deleted.</param>
    protected TypeBase(nint handle, bool isDisposed)
        : base(handle)
    {
        IsDisposed = isDisposed;
    }
}
