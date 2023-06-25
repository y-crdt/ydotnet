namespace YDotNet.Infrastructure;

internal static class MemoryReader
{
    internal static unsafe byte[] ReadBytes(nint handle, uint length)
    {
        var data = new byte[length];
        var stream = new UnmanagedMemoryStream((byte*) handle.ToPointer(), length);
        int bytesRead;

        do
        {
            bytesRead = stream.Read(data, offset: 0, data.Length);
        }
        while (bytesRead < data.Length);

        stream.Dispose();

        return data;
    }

    internal static byte[]? TryReadBytes(nint handle, uint length)
    {
        return handle == nint.Zero ? null : ReadBytes(handle, length);
    }
}
