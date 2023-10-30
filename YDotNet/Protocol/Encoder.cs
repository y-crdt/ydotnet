using System.Buffers;
using System.Text;

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
    public async ValueTask WriteVarUintAsync(ulong value, CancellationToken ct = default)
    {
        do
        {
            byte lower7bits = (byte)(value & 0x7f);

            value >>= 7;

            if (value > 0)
            {
                lower7bits |= 128;
            }

            await WriteByteAsync(lower7bits, ct);
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
    public async ValueTask WriteVarUint8Array(byte[] value, CancellationToken ct = default)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        await WriteVarUintAsync((ulong)value.Length, ct);
        await WriteBytesAsync(value, ct);
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
    public async ValueTask WriteVarStringAsync(string value, CancellationToken ct = default)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var length = Encoding.UTF8.GetByteCount(value);
        if (length > stringBuffer.Length)
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
            await WriteCoreAsync(value, stringBuffer, ct);
        }

        async Task WriteCoreAsync(string value, byte[] buffer, CancellationToken ct)
        {
            var length = Encoding.UTF8.GetBytes(value, buffer);

            await WriteVarUintAsync((ulong)length, ct);
            await WriteBytesAsync(new ArraySegment<byte>(buffer, 0, length), ct);
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
    protected abstract ValueTask WriteByteAsync(byte value, CancellationToken ct);

    /// <summary>
    /// Write a byte array.
    /// </summary>
    /// <param name="bytes">The byte array to write.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task representing the async operation.
    /// </returns>
    protected abstract ValueTask WriteBytesAsync(ArraySegment<byte> bytes, CancellationToken ct);
}
