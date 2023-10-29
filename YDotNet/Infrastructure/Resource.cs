namespace YDotNet.Infrastructure;

/// <summary>
///     Base class for managed resources that hold native resources.
/// </summary>
public abstract class Resource : IDisposable
{
    internal Resource(bool isDisposed)
    {
        IsDisposed = isDisposed;
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this instance is disposed.
    /// </summary>
    public bool IsDisposed { get; protected set; }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Marks the object as disposed to stop all further calls.
    /// </summary>
    internal void MarkDisposed()
    {
        IsDisposed = true;
    }

    /// <summary>
    ///     Releases all unmanaged resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> if managed resources should be disposed as well.</param>
    protected abstract void DisposeCore(bool disposing);

    /// <summary>
    ///     Throws an exception if this object or the owner is disposed.
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
    ///     Releases all unmanaged resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> if managed resources should be disposed as well.</param>
    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        DisposeCore(disposing);

        IsDisposed = true;
    }
}
