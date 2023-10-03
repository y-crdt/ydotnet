using System.Threading.Channels;

namespace YDotNet.Server.Redis.Internal;

public interface ICanEstimateSize
{
    int EstimateSize();
}

public sealed class PublishQueue<T> where T : ICanEstimateSize
{
    private readonly Channel<object> inputChannel = Channel.CreateBounded<object>(100);
    private readonly Channel<List<T>> outputChannel = Channel.CreateBounded<List<T>>(2);
    private readonly CancellationTokenSource cts = new();

    public PublishQueue(int maxCount, int maxSize, int timeout, Func<List<T>, CancellationToken, Task> handler)
    {
        Task.Run(async () =>
        {
            var batchList = new List<T>(maxCount);
            var batchSize = 0;

            // Just a marker object to force sending out new batches.
            var force = new object();

            await using var timer = new Timer(_ => inputChannel.Writer.TryWrite(force));

            async Task TrySendAsync()
            {
                if (batchList.Count > 0)
                {
                    await outputChannel.Writer.WriteAsync(batchList, cts.Token);

                    // Create a new batch, because the value is shared and might be processes by another concurrent task.
                    batchList = new List<T>();
                    batchSize = 0;
                }
            }

            // Exceptions usually that the process was stopped and the channel closed, therefore we do not catch them.
            await foreach (var item in inputChannel.Reader.ReadAllAsync(cts.Token))
            {
                if (ReferenceEquals(item, force))
                {
                    // Our item is the marker object from the timer.
                    await TrySendAsync();
                }
                else if (item is T typed)
                {
                    // The timeout restarts with the last event and should push events out if no further events are received.
                    timer.Change(timeout, Timeout.Infinite);

                    batchList.Add(typed);
                    batchSize += typed.EstimateSize();

                    if (batchList.Count >= maxSize || batchSize >= maxSize)
                    {
                        await TrySendAsync();
                    }
                }
            }

            await TrySendAsync();
        }, cts.Token).ContinueWith(x => outputChannel.Writer.TryComplete(x.Exception));

        Task.Run(async () =>
        {
            await foreach (var batch in outputChannel.Reader.ReadAllAsync(cts.Token))
            {
                await handler(batch, cts.Token);
            }
        }, cts.Token);
    }

    public ValueTask EnqueueAsync(T item,
        CancellationToken ct)
    {
        return inputChannel.Writer.WriteAsync(item, ct);
    }

    public void Dispose()
    {
        cts.Cancel();
    }
}
