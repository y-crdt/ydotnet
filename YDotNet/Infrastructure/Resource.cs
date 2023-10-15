namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for all resoruces.
/// </summary>
public abstract class Resource : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether this instance is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Throws an exception if this object or the owner is disposed.
    /// </summary>
    /// <exception cref="ObjectDisposedException">Object or the owner has been disposed.</exception>
    protected void ThrowIfDisposed()
    {
        if (IsDisposed)
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
