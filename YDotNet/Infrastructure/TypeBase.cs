namespace YDotNet.Infrastructure;

/// <summary>
/// Base class for all managed types.
/// </summary>
public abstract class TypeBase : ITypeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeBase"/> class.
    /// </summary>
    /// <param name="isDeleted">A value indicating if the instance is deleted.</param>
    protected TypeBase(bool isDeleted)
    {
        IsDeleted = isDeleted;
    }

    /// <inheritdoc/>
    public bool IsDeleted { get; private set; }

    /// <summary>
    /// Throws an exception if the type is deleted.
    /// </summary>
    protected void ThrowIfDeleted()
    {
        if (IsDeleted)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    /// <inheritdoc/>
    public void MarkDeleted()
    {
        IsDeleted = true;
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

    /// <summary>
    /// Gets a value indicating whether the instance is deleted.
    /// </summary>
    bool IsDeleted { get; }
}
