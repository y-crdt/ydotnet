namespace YDotNet.Infrastructure;

public abstract class TypeBase : ITypeBase
{
    private bool isDeleted;

    protected void ThrowIfDeleted()
    {

    }

    public void MarkDeleted()
    {
        throw new NotImplementedException();
    }
}

public interface ITypeBase
{
    /// <summary>
    /// Marks the object as deleted to stop all further calls.
    /// </summary>
    void MarkDeleted();
}
