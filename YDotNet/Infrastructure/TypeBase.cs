namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for all managed types.
/// </summary>
public abstract class TypeBase : ITypeBase
{
    private bool isDeleted;

    /// <summary>
    /// Throws an exception if the type is deleted.
    /// </summary>
    protected void ThrowIfDeleted()
    {
        if (isDeleted)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <inheritdoc/>
    public void MarkDeleted()
    {
        isDeleted = true;
    }
}

/// <summary>
/// Base class for all managed types.
/// </summary>
public interface ITypeBase
{
    /// <summary>
    /// Marks the object as deleted to stop all further calls.
    /// </summary>
    void MarkDeleted();
}
