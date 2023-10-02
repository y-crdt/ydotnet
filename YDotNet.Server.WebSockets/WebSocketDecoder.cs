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

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(buffer);
    }

    protected override async ValueTask<byte> ReadByteAsync(
        CancellationToken ct)
    {
        EnsureSocketIsOpen();

        await ReadIfEndOfBufferReachedAsync(ct);

        return buffer[bufferIndex++];
    }

    protected override async ValueTask ReadBytesAsync(Memory<byte> memory,
        CancellationToken ct)
    {
        EnsureSocketIsOpen();

        while (memory.Length > 0)
        {
            await ReadIfEndOfBufferReachedAsync(ct);

            var bytesLeft = bufferLength - bufferIndex;
            var bytesToCopy = Math.Min(memory.Length, bytesLeft);

            buffer.AsMemory(bufferIndex, bytesToCopy).CopyTo(memory);
            memory = memory.Slice(bytesToCopy);

            bufferIndex += bytesToCopy;
        }
    }

    private async Task ReadIfEndOfBufferReachedAsync(CancellationToken ct)
    {
        if (bufferIndex == bufferLength)
        {
            var received = await webSocket.ReceiveAsync(buffer, ct);

            bufferLength = received.Count;
            bufferIndex = 0;
        }
    }

    private void EnsureSocketIsOpen()
    {
        if (webSocket.State != WebSocketState.Open)
        {
            throw new InvalidOperationException("Socket is already closed.");
        }
    }
}
