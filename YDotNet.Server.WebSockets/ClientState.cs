using System.Net.WebSockets;

namespace YDotNet.Server.WebSockets;

public sealed class ClientState : IDisposable
{
    private readonly SemaphoreSlim slimLock = new(1);

    required public WebSocket WebSocket { get; set; }

    required public WebSocketEncoder Encoder { get; set; }

    required public WebSocketDecoder Decoder { get; set; }

    required public DocumentContext DocumentContext { get; set; }

    required public string DocumentName { get; init; }

    public bool IsSynced { get; set; }

    public Queue<byte[]> PendingUpdates { get; } = new Queue<byte[]>();

    public async Task WriteLockedAsync<T>(T state, Func<WebSocketEncoder, T, ClientState, CancellationToken, Task> action, CancellationToken ct)
    {
        await slimLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            await action(Encoder, state, this, ct).ConfigureAwait(false);
        }
        finally
        {
            slimLock.Release();
        }
    }

    public void Dispose()
    {
        WebSocket.Dispose();

        Encoder.Dispose();
        Decoder.Dispose();
    }
}
