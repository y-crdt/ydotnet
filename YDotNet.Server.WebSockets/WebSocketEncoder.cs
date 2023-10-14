using System.Net.WebSockets;
using YDotNet.Protocol;

namespace YDotNet.Server.WebSockets;

public sealed class WebSocketEncoder : Encoder, IDisposable
{
    private readonly byte[] buffer = new byte[1];
    private readonly WebSocket webSocket;

    public WebSocketEncoder(WebSocket webSocket)
    {
        this.webSocket = webSocket;
    }

    public void Dispose()
    {
    }

    protected override ValueTask WriteByteAsync(byte value,
        CancellationToken ct)
    {
        buffer[0] = value;

        return new ValueTask(webSocket.SendAsync(buffer, WebSocketMessageType.Binary, false, ct));
    }

    protected override ValueTask WriteBytesAsync(ArraySegment<byte> bytes,
        CancellationToken ct)
    {
        return new ValueTask(webSocket.SendAsync(bytes, WebSocketMessageType.Binary, false, ct));
    }

    public ValueTask EndMessageAsync(
        CancellationToken ct = default)
    {
        return new ValueTask(webSocket.SendAsync(Array.Empty<byte>(), WebSocketMessageType.Binary, true, ct));
    }
}
