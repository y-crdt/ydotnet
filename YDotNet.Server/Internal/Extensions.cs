using YDotNet.Document;
using YDotNet.Document.Transactions;
using YDotNet.Protocol;

namespace YDotNet.Server.Internal;

internal static class Extensions
{
    public static Transaction ReadTransactionOrThrow(this Doc doc)
    {
        return doc.ReadTransaction() ?? throw new InvalidOperationException("Failed to open transaction.");
    }

    public static Transaction WriteTransactionOrThrow(this Doc doc)
    {
        return doc.WriteTransaction() ?? throw new InvalidOperationException("Failed to open transaction.");
    }

    public static async ValueTask<byte[]> GetUpdateArray(this byte[] source)
    {
        return source;
        var decoder = new MemoryDecoder(source);
        await decoder.ReadVarUintAsync();
        await decoder.ReadVarUintAsync();

        return await decoder.ReadVarUint8ArrayAsync(default);
    }

    class MemoryDecoder : Decoder
    {
        private readonly byte[] source;
        private int position = 0;

        public MemoryDecoder(byte[] source)
        {
            this.source = source;
        }

        protected override ValueTask<byte> ReadByteAsync(
            CancellationToken ct)
        {
            if (position == source.Length)
            {
                throw new InvalidOperationException("End of buffer reached.");
            }

            return new ValueTask<byte>(source[position++]);
        }

        protected override ValueTask ReadBytesAsync(Memory<byte> bytes, 
            CancellationToken ct)
        {
            if (position + bytes.Length >= source.Length)
            {
                throw new InvalidOperationException("End of buffer reached.");
            }

            source.AsMemory(position, bytes.Length).CopyTo(bytes);
            position += bytes.Length;

            return default;
        }
    }
}
