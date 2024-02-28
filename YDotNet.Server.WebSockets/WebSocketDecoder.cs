using System.Buffers;
using System.Net.WebSockets;
using YDotNet.Protocol;

namespace YDotNet.Server.WebSockets;

public sealed class WebSocketDecoder : Decoder, IDisposable
{
    private readonly WebSocket webSocket;
    private readonly byte[] buffer = ArrayPool<byte>.Shared.Rent(1024 * 4);
    private int bufferLength;
    private int bufferIndex;

    public WebSocketDecoder(WebSocket webSocket)
    {
        this.webSocket = webSocket;
    }

    public bool CanRead { get; private set; } = true;

    public bool HasMore => bufferIndex < bufferLength;

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }

    protected override async ValueTask<byte> ReadByteAsync(CancellationToken ct)
    {
        EnsureSocketIsOpen();

        await ReadIfEndOfBufferReachedAsync(ct).ConfigureAwait(false);

        return buffer[bufferIndex++];
    }

    protected override async ValueTask ReadBytesAsync(Memory<byte> memory, CancellationToken ct)
    {
        EnsureSocketIsOpen();

        while (memory.Length > 0)
        {
            await ReadIfEndOfBufferReachedAsync(ct).ConfigureAwait(false);

            var bytesLeft = bufferLength - bufferIndex;
            var bytesToCopy = Math.Min(memory.Length, bytesLeft);

            buffer.AsMemory(bufferIndex, bytesToCopy).CopyTo(memory);
            memory = memory[bytesToCopy..];

            bufferIndex += bytesToCopy;
        }
    }

    private async Task ReadIfEndOfBufferReachedAsync(CancellationToken ct)
    {
        if (bufferIndex == bufferLength)
        {
            var received = await webSocket.ReceiveAsync(buffer, ct).ConfigureAwait(false);

            if (received.CloseStatus != null)
            {
                CanRead = false;
                throw new WebSocketException("Socket is already closed.");
            }

            bufferLength = received.Count;
            bufferIndex = 0;
        }
    }

    private void EnsureSocketIsOpen()
    {
        if (webSocket.State != WebSocketState.Open && CanRead)
        {
            throw new WebSocketException("Socket is already closed.");
        }
    }
}
