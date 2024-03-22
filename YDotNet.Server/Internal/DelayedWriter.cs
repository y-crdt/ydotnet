using System.Reactive.Concurrency;
using System.Reactive.Disposables;
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
        completion = ThrottleMax(writes, delayTime, delayMax)
            .Select(m => Observable.FromAsync(action))
            .Concat()
            .ToTask();
    }

    public Task FlushAsync()
    {
        writes.OnNext(value: true);
        writes.OnCompleted();
        return completion;
    }

    public void Ping()
    {
        writes.OnNext(value: true);
    }

    private static IObservable<T> ThrottleMax<T>(IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
    {
        var scheduler = Scheduler.Default;

        return Observable.Create<T>(o =>
        {
            var lastValue = default(T);
            var lastWrite = scheduler.Now;
            var dueTimeDisposable = new SerialDisposable();

            void Next()
            {
                dueTimeDisposable.Disposable = Disposable.Empty;

                o.OnNext(lastValue!);
                lastValue = default;
                lastWrite = scheduler.Now;
            }

            return source.Subscribe(
                x =>
                {
                    if (scheduler.Now - lastWrite > maxTime && dueTimeDisposable.Disposable != Disposable.Empty)
                    {
                        Next();
                    }
                    else
                    {
                        lastValue = x;
                        dueTimeDisposable.Disposable = scheduler.Schedule(dueTime, Next);
                    }
                },
                o.OnError,
                o.OnCompleted);
        });
    }
}
