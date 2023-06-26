using System.Runtime.InteropServices;
using System.Text;

namespace YDotNet.Infrastructure;

internal static class MemoryWriter
{
    public static unsafe (Stream Stream, nint Pointer) WriteUtf8String(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value + "\0");
        var pointer = Marshal.AllocHGlobal(bytes.Length);

        using var stream = new UnmanagedMemoryStream(
            (byte*) pointer.ToPointer(),
            length: 0,
            bytes.Length,
            FileAccess.Write
        );

        stream.Write(bytes);

        return (stream, pointer);
    }
}
