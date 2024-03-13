using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Protocol;

namespace YDotNet.Server.Internal;

internal static class Extensions
{
    public static IObservable<T> ThrottleMax<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime)
    {
        return source.ThrottleMax(dueTime, maxTime, Scheduler.Default);
    }

    public static IObservable<T> ThrottleMax<T>(this IObservable<T> source, TimeSpan dueTime, TimeSpan maxTime, IScheduler scheduler)
    {
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

    public static Transaction ReadTransactionOrThrow(this Doc doc)
    {
        return doc.ReadTransaction() ?? throw new InvalidOperationException("Failed to open transaction.");
    }

    public static Transaction WriteTransactionOrThrow(this Doc doc)
    {
        return doc.WriteTransaction() ?? throw new InvalidOperationException("Failed to open transaction.");
    }

    private sealed class MemoryDecoder : Decoder
    {
        private readonly byte[] source;
        private int position = 0;

        public MemoryDecoder(byte[] source)
        {
            this.source = source;
        }

        protected override ValueTask<byte> ReadByteAsync(
            CancellationToken ct)
        {
            if (position == source.Length)
            {
                throw new InvalidOperationException("End of buffer reached.");
            }

            return new ValueTask<byte>(source[position++]);
        }

        protected override ValueTask ReadBytesAsync(
            Memory<byte> bytes,
            CancellationToken ct)
        {
            if (position + bytes.Length >= source.Length)
            {
                throw new InvalidOperationException("End of buffer reached.");
            }

            source.AsMemory(position, bytes.Length).CopyTo(bytes);
            position += bytes.Length;

            return default;
        }
    }
}
