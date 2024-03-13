using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;

namespace YDotNet.Server.Internal;

internal sealed class DelayedWriter
{
    private readonly Subject<bool> writes = new();
    private readonly Task completion;

    public DelayedWriter(TimeSpan delayTime, TimeSpan delayMax, Func<Task> action)
    {
        completion = writes
            .ThrottleMax(delayTime, delayMax)
            .Select(m => Observable.FromAsync(action))
            .Concat()
            .ToTask();
    }

    public Task FlushAsync()
    {
        writes.OnCompleted();
        return completion;
    }

    public void Ping()
    {
        writes.OnNext(value: true);
    }
}
