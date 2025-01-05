namespace YDotNet.Server.Internal;

public sealed class DelegateDisposable(Action callback) : IDisposable
{
    public void Dispose()
    {
        callback();
    }
}
