namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for all managed types.
/// </summary>
public abstract class TypeBase : ITypeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeBase"/> class.
    /// </summary>
    /// <param name="isDisposed">A value indicating if the instance is deleted.</param>
    protected TypeBase(bool isDisposed)
    {
        IsDisposed = isDisposed;
    }

    /// <inheritdoc/>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Throws an exception if the type is disposed.
    /// </summary>
    protected void ThrowIfDisposed()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <inheritdoc/>
    public void MarkDisposed()
    {
        IsDisposed = true;
    }
}

/// <summary>
/// Base class for all managed types.
/// </summary>
public interface ITypeBase
{
    /// <summary>
    /// Marks the object as disposed to stop all further calls.
    /// </summary>
    void MarkDisposed();

    /// <summary>
    /// Gets a value indicating whether the instance is disposed.
    /// </summary>
    bool IsDisposed { get; }
}
