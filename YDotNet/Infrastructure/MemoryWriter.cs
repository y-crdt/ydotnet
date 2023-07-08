using System.Runtime.InteropServices;
using System.Text;

namespace YDotNet.Infrastructure;

internal static class MemoryWriter
{
    internal static unsafe (Stream Stream, nint Pointer) WriteUtf8String(string value)
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

    internal static nint WriteUtf8StringArray(string[] values)
    {
        var size = Marshal.SizeOf<nint>();
        var pointer = Marshal.AllocHGlobal(size * values.Length);

        for (var i = 0; i < values.Length; i++)
        {
            // TODO [LSViana] Free the memory allocated here.
            Marshal.WriteIntPtr(pointer + i * size, WriteUtf8String(values[i]).Pointer);
        }

        return pointer;
    }

    internal static nint WriteStructArray<T>(T[] value)
        where T : struct
    {
        var size = Marshal.SizeOf<T>();
        var handle = Marshal.AllocHGlobal(size * value.Length);

        for (var i = 0; i < value.Length; i++)
        {
            Marshal.StructureToPtr(value[i], handle + i * size, fDeleteOld: false);
        }

        return handle;
    }
}
