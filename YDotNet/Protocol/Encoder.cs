using System.Buffers;
using System.Text;

#pragma warning disable SA1116 // Split parameters should start on line after declaration

namespace YDotNet.Protocol;

/// <summary>
/// Base class for all encoders for implementing the y-protocol.
/// </summary>
public abstract class Encoder
{
    private readonly byte[] stringBuffer = new byte[128];

    /// <summary>
    /// Writes a number with variable length encoding.
    /// </summary>
    /// <param name="value">The number to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    public async ValueTask WriteVarUintAsync(long value,
        CancellationToken ct = default)
    {
        do
        {
            byte lower7bits = (byte)(value & 0x7f);

            value >>= 7;

            if (value > 0)
            {
                lower7bits |= 128;
            }

            await this.WriteByteAsync(lower7bits, ct);
        }
        while (value > 0);
    }

    /// <summary>
    /// Writes a byte array.
    /// </summary>
    /// <param name="value">The byte array to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public async ValueTask WriteVarUint8Array(byte[] value,
        CancellationToken ct = default)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        await this.WriteVarUintAsync(value.Length, ct);
        await this.WriteBytesAsync(value, ct);
    }

    /// <summary>
    /// Writes a string.
    /// </summary>
    /// <param name="value">The string to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is null.</exception>
    public async ValueTask WriteVarStringAsync(string value,
        CancellationToken ct = default)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var length = Encoding.UTF8.GetByteCount(value);
        if (length > this.stringBuffer.Length)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                await WriteCoreAsync(value, buffer, ct);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        else
        {
            await WriteCoreAsync(value, this.stringBuffer, ct);
        }

        async Task WriteCoreAsync(string value, byte[] buffer, CancellationToken ct)
        {
            var length = Encoding.UTF8.GetBytes(value, buffer);

            await this.WriteBytesAsync(new ArraySegment<byte>(buffer, 0, length), ct);
        }
    }

    /// <summary>
    /// Write a single byte.
    /// </summary>
    /// <param name="value">The byte to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    protected abstract ValueTask WriteByteAsync(byte value,
        CancellationToken ct);

    /// <summary>
    /// Write a byte array.
    /// </summary>
    /// <param name="bytes">The byte array to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    protected abstract ValueTask WriteBytesAsync(ArraySegment<byte> bytes,
        CancellationToken ct);
}
