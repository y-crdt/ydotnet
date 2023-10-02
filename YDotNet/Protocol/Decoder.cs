using System;
using System.Buffers;
using System.Text;

namespace YDotNet.Protocol;

public abstract class Decoder
{
    private readonly byte[] stringBuffer = new byte[128];

    public abstract long Position { get; }

    public async ValueTask<long> ReadVarUintAsync(
        CancellationToken ct = default)
    {
        int value = 0, shift = 0;

        while (true)
        {
            var lower7bits = await ReadByteAsync(ct);

            value |= (lower7bits & 0x7f) << shift;

            if ((lower7bits & 128) == 0)
            {
                return value;
            }

            shift += 7;
        }

        throw new IndexOutOfRangeException();
    }

    public async ValueTask<byte[]> ReadVarUint8ArrayAsync(
        CancellationToken ct)
    {
        var arrayLength = await ReadVarUintAsync(ct);
        var arrayBuffer = new byte[arrayLength];

        await ReadBytesAsync(arrayBuffer, ct);

        return arrayBuffer;
    }

    public async ValueTask<byte[]> ReadVarUint8ArrayWithLengthAsync(
        CancellationToken ct)
    {
        var positionBeforeLength = Position;

        // Reserve enough place for the array that we have just read and the total array.
        var arrayLength = await ReadVarUintAsync(ct);
        var arrayPrefix = (int)(Position - positionBeforeLength);
        var arrayBuffer = new byte[arrayLength + arrayPrefix];

        // We cannot read backwards, therefore we have to write the number to the array again.
        WriteWithVariableEncoding(arrayLength, arrayBuffer);

        await ReadBytesAsync(arrayBuffer.AsMemory().Slice(arrayPrefix), ct);

        static void WriteWithVariableEncoding(long value, byte[] target)
        {
            var index = 0;
            do
            {
                byte lower7bits = (byte)(value & 0x7f);

                value >>= 7;

                if (value > 0)
                {
                    lower7bits |= 128;
                }

                target[index++] = lower7bits;
            }
            while (value > 0);
        }

        return arrayBuffer;
    }

    public async ValueTask<string> ReadVarStringAsync(
        CancellationToken ct)
    {
        var length = (int)await ReadVarUintAsync(ct);
        if (length > stringBuffer.Length)
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
            return await ReadCoreAsync(length, stringBuffer, ct);
        }

        async ValueTask<string> ReadCoreAsync(int length, byte[] buffer, CancellationToken ct)
        {
            var slicedBuffer = buffer.AsMemory(0, length);
            await ReadBytesAsync(slicedBuffer, ct);

            return Encoding.UTF8.GetString(slicedBuffer.Span);
        }
    }

    protected abstract ValueTask<byte> ReadByteAsync(
        CancellationToken ct);

    protected abstract ValueTask ReadBytesAsync(Memory<byte> bytes,
        CancellationToken ct);
}
