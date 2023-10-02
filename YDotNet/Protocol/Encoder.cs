using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YDotNet.Protocol;

public abstract class Encoder
{
    private const int BITS7 = 1 << 7;
    private const int BITS8 = 1 << 8;
    private readonly byte[] stringBuffer = new byte[128];

    public async ValueTask WriteVarUintAsync(long value,
        CancellationToken ct = default)
    {
        while (value > BITS7)
        {
            await WriteByteAsync((byte)(BITS8 | (BITS7 & value)), ct);
            value >>= 7;
        }

        await WriteByteAsync((byte)(BITS7 & value), ct);
    }

    public async ValueTask WriteVarUint8Array(byte[] value,
        CancellationToken ct = default)
    {
        await WriteVarUintAsync(value.Length, ct);
        await WriteBytesAsync(value, ct);
    }

    public async ValueTask WriteVarStringAsync(string value,
        CancellationToken ct = default)
    {
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

            await WriteBytesAsync(new ArraySegment<byte>(buffer, 0, length), ct);
        }
    }

    public abstract ValueTask EndMessageAsync(
        CancellationToken ct = default);

    protected abstract ValueTask WriteByteAsync(byte value,
        CancellationToken ct);

    protected abstract ValueTask WriteBytesAsync(ArraySegment<byte> bytes,
        CancellationToken ct);
}
