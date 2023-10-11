namespace YDotNet.Infrastructure;

public abstract class Resource : IResourceOwner, IDisposable
{
    protected Resource(IResourceOwner? owner = null)
    {
        this.Owner = owner;
    }

    public bool IsDisposed { get; private set; }

    public IResourceOwner? Owner { get; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    protected void ThrowIfDisposed()
    {
        if (IsDisposed || Owner?.IsDisposed == true)
        {
            throw new ObjectDisposedException(GetType().Name);
        }
    }

    protected void Dispose(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        DisposeCore(disposing);
        IsDisposed = true;
    }

    protected abstract void DisposeCore(bool disposing);
}
