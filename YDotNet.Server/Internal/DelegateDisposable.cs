namespace YDotNet.Server.Internal;

public sealed class DelegateDisposable : IDisposable
{
    private readonly Action callback;

    public DelegateDisposable(Action callback)
    {
        this.callback = callback;
    }

    public void Dispose()
    {
        callback();
    }
}
