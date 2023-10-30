namespace YDotNet.Protocol;

/// <summary>
/// Write to a buffer.
/// </summary>
public sealed class BufferEncoder : Encoder
{
    private readonly MemoryStream buffer = new();

    /// <summary>
    /// Gets the content of the buffer.
    /// </summary>
    /// <returns>The content of the buffer.</returns>
    public byte[] ToArray()
    {
        return buffer.ToArray();
    }

    /// <inheritdoc/>
    protected override ValueTask WriteByteAsync(byte value, CancellationToken ct)
    {
        buffer.WriteByte(value);
        return default;
    }

    /// <inheritdoc/>
    protected override ValueTask WriteBytesAsync(ArraySegment<byte> bytes, CancellationToken ct)
    {
        buffer.Write(bytes);
        return default;
    }
}
