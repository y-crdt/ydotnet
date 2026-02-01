using System.Net.WebSockets;
using YDotNet.Protocol;

namespace YDotNet.Server.WebSockets;

public sealed class WebSocketEncoder(WebSocket webSocket) : Encoder, IDisposable
{
    private readonly byte[] buffer = new byte[1];

    public void Dispose()
    {
    }

    public override ValueTask FlushAsync(CancellationToken ct = default)
    {
        return new ValueTask(webSocket.SendAsync(Array.Empty<byte>(), WebSocketMessageType.Binary, endOfMessage: true, ct));
    }

    protected override ValueTask WriteByteAsync(byte value, CancellationToken ct)
    {
        buffer[0] = value;

        return new ValueTask(webSocket.SendAsync(buffer, WebSocketMessageType.Binary, endOfMessage: false, ct));
    }

    protected override ValueTask WriteBytesAsync(ArraySegment<byte> bytes, CancellationToken ct)
    {
        return new ValueTask(webSocket.SendAsync(bytes, WebSocketMessageType.Binary, endOfMessage: false, ct));
    }
}
