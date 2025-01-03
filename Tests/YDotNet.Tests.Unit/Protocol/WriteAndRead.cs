using NUnit.Framework;
using YDotNet.Infrastructure;
using YDotNet.Protocol;

namespace YDotNet.Tests.Unit.Protocol;

public class WriteAndRead
{
    [Test]
    [TestCase(0ul)]
    [TestCase(1ul)]
    [TestCase(3826503548ul)]
    [TestCase(ClientIdGenerator.MaxSafeInteger)]
    public async Task EncodeAndDecodeInt(ulong input)
    {
        // Arrange
        var encoder = new BufferEncoder();

        // Act
        await encoder.WriteVarUintAsync(input);
        await encoder.WriteVarUintAsync(1000ul); // They should not read pass through.

        var decoder = new BufferDecoder(encoder.ToArray());
        var read = await decoder.ReadVarUintAsync();

        // Assert

        Assert.That(read, Is.EqualTo(input));
    }

    [Test]
    [TestCase("")]
    [TestCase("Hello YDotNet")]
    public async Task EncodeAndDecodeString(string input)
    {
        // Arrange
        var encoder = new BufferEncoder();

        // Act
        await encoder.WriteVarStringAsync(input);
        await encoder.WriteVarUintAsync(1000ul); // They should not read pass through.

        var decoder = new BufferDecoder(encoder.ToArray());
        var read = await decoder.ReadVarStringAsync();

        // Assert

        Assert.That(read, Is.EqualTo(input));
    }

    [Test]
    [TestCase(0)]
    [TestCase(100)]
    [TestCase(1000)]
    public async Task EncodeAndDecodeBytes(int length)
    {
        var input = Bytes(length);

        // Arrange
        var encoder = new BufferEncoder();

        // Act
        await encoder.WriteVarUint8Array(input);
        await encoder.WriteVarUintAsync(1000ul); // They should not read pass through.

        var decoder = new BufferDecoder(encoder.ToArray());
        var read = await decoder.ReadVarUint8ArrayAsync();

        // Assert
        Assert.That(read, Is.EqualTo(input));
    }

    [Test]
    public async Task DecodeJsSample()
    {
        // Arrange
        var buffer = new byte[]
        {
            1,
            252,
            158,
            207,
            160,
            14,
            4,
            56,
            123,
            34,
            117,
            115,
            101,
            114,
            34,
            58,
            123,
            34,
            114,
            97,
            110,
            100,
            111,
            109,
            34,
            58,
            48,
            46,
            55,
            49,
            55,
            57,
            54,
            54,
            50,
            55,
            50,
            52,
            57,
            55,
            55,
            54,
            57,
            54,
            44,
            34,
            109,
            101,
            115,
            115,
            97,
            103,
            101,
            34,
            58,
            34,
            72,
            101,
            108,
            108,
            111,
            34,
            125,
            125
        };

        var decoder = new BufferDecoder(buffer);

        // Act
        var clientCount = await decoder.ReadVarUintAsync();

        var client1Id = await decoder.ReadVarUintAsync();
        var client1Clock = await decoder.ReadVarUintAsync();
        var client1State = await decoder.ReadVarStringAsync();

        // Assert
        Assert.That(clientCount, Is.EqualTo(1));
        Assert.That(client1Id, Is.EqualTo(3826503548));
        Assert.That(client1Clock, Is.EqualTo(4));
        Assert.That(client1State, Is.EqualTo("{\"user\":{\"random\":0.7179662724977696,\"message\":\"Hello\"}}"));
    }

    private byte[] Bytes(int length)
    {
        var result = new byte[length];

        for (var i = 0; i < length; i++)
        {
            result[i] = (byte)(i % 255);
        }

        return result;
    }

    class BufferDecoder(byte[] buffer) : Decoder
    {
        private readonly Stream stream = new MemoryStream(buffer);

        protected override ValueTask<byte> ReadByteAsync(
            CancellationToken ct)
        {
            var result = stream.ReadByte();

            if (result < 0)
            {
                throw new InvalidOperationException("End of stream reached.");
            }

            return new ValueTask<byte>((byte)result);
        }

        protected override async ValueTask ReadBytesAsync(Memory<byte> bytes,
            CancellationToken ct)
        {
            await stream.ReadAsync(bytes, ct);
        }
    }
}
