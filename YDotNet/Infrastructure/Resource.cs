namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for all resoruces.
/// </summary>
public abstract class Resource : IResourceOwner, IDisposable
{
    internal Resource(IResourceOwner? owner = null)
    {
        Owner = owner;
    }

    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Gets the owner of this resource.
    /// </summary>
    public IResourceOwner? Owner { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    /// <summary>
    /// Throws an exception if this object or the owner is disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Object or the owner has been disposed.</exception>
    protected void ThrowIfDisposed()
    {
        if (IsDisposed || Owner?.IsDisposed == true)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <summary>
    /// Releases all unmanaged resources.
    /// </summary>
    /// <param name="disposing">True, if also managed resources should be disposed.</param>
    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        DisposeCore(disposing);
        IsDisposed = true;
    }

    /// <summary>
    /// Releases all unmanaged resources.
    /// </summary>
    /// <param name="disposing">True, if also managed resources should be disposed.</param>
    protected internal abstract void DisposeCore(bool disposing);
}
