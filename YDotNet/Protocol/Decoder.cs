using System.Buffers;
using System.Text;

namespace YDotNet.Protocol;

/// <summary>
/// Base class for all decoders for implementing the y-protocol.
/// </summary>
public abstract class Decoder
{
    private readonly byte[] stringBuffer = new byte[128];

    /// <summary>
    /// Reads an unsigned integer with variable length encoding.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The decoded integer.
    /// </returns>
    /// <exception cref="IndexOutOfRangeException">The input is in an invalid format.</exception>
    public async ValueTask<long> ReadVarUintAsync(
        CancellationToken ct = default)
    {
        int value = 0, shift = 0;

        while (true)
        {
            var lower7bits = await this.ReadByteAsync(ct);

            value |= (lower7bits & 0x7f) << shift;

            if ((lower7bits & 128) == 0)
            {
                return value;
            }

            shift += 7;
        }

        throw new IndexOutOfRangeException();
    }

    /// <summary>
    /// Reads the length and the bytes of an array.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The decoded byte array.
    /// </returns>
    public async ValueTask<byte[]> ReadVarUint8ArrayAsync(
        CancellationToken ct)
    {
        var arrayLength = await this.ReadVarUintAsync(ct);
        var arrayBuffer = new byte[arrayLength];

        await this.ReadBytesAsync(arrayBuffer, ct);

        return arrayBuffer;
    }

    /// <summary>
    /// Reads an UTF8 string.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The decoded string.
    /// </returns>
    public async ValueTask<string> ReadVarStringAsync(
        CancellationToken ct)
    {
        var length = (int)await this.ReadVarUintAsync(ct);
        if (length > this.stringBuffer.Length)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                return await ReadCoreAsync(length, buffer, ct);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        else
        {
            return await ReadCoreAsync(length, this.stringBuffer, ct);
        }

        async ValueTask<string> ReadCoreAsync(int length, byte[] buffer, CancellationToken ct)
        {
            var slicedBuffer = buffer.AsMemory(0, length);
            await this.ReadBytesAsync(slicedBuffer, ct);

            return Encoding.UTF8.GetString(slicedBuffer.Span);
        }
    }

    /// <summary>
    /// Reads the next byte.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The read byte.
    /// </returns>
    protected abstract ValueTask<byte> ReadByteAsync(
        CancellationToken ct);

    /// <summary>
    /// Reads the bytes to the given memory.
    /// </summary>
    /// <param name="bytes">The bytes to read.</param>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The task that completes when everything has been read.
    /// </returns>
    protected abstract ValueTask ReadBytesAsync(Memory<byte> bytes,
        CancellationToken ct);
}
