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
    public async ValueTask<ulong> ReadVarUintAsync(
        CancellationToken ct = default)
    {
        var resultSum = 0ul;
        var resultShift = 0;

        while (true)
        {
            var lower7bits = (ulong)await ReadByteAsync(ct).ConfigureAwait(false);

            resultSum |= (lower7bits & 0x7f) << resultShift;

            if ((lower7bits & 128) == 0)
            {
                return resultSum;
            }

            resultShift += 7;
        }

#pragma warning disable MA0012 // Do not raise reserved exception type
        throw new IndexOutOfRangeException();
#pragma warning restore MA0012 // Do not raise reserved exception type
    }

    /// <summary>
    /// Reads the length and the bytes of an array.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    /// <returns>
    /// The decoded byte array.
    /// </returns>
    public async ValueTask<byte[]> ReadVarUint8ArrayAsync(
        CancellationToken ct = default)
    {
        var arrayLength = await ReadVarUintAsync(ct).ConfigureAwait(false);
        var arrayBuffer = new byte[arrayLength];

        await ReadBytesAsync(arrayBuffer, ct).ConfigureAwait(false);

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
        CancellationToken ct = default)
    {
        var length = (int)await ReadVarUintAsync(ct).ConfigureAwait(false);
        if (length > stringBuffer.Length)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                return await ReadCoreAsync(length, buffer, ct).ConfigureAwait(false);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
        else
        {
            return await ReadCoreAsync(length, stringBuffer, ct).ConfigureAwait(false);
        }

        async ValueTask<string> ReadCoreAsync(int length, byte[] buffer, CancellationToken ct)
        {
            var slicedBuffer = buffer.AsMemory(0, length);
            await ReadBytesAsync(slicedBuffer, ct).ConfigureAwait(false);

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
    protected abstract ValueTask ReadBytesAsync(Memory<byte> bytes, CancellationToken ct);
}
