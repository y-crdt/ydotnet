using System.Runtime.InteropServices;
using System.Text;

namespace YDotNet.Infrastructure;

internal static class MemoryWriter
{
    internal static unsafe nint WriteUtf8String(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value + "\0");
        var pointer = Marshal.AllocHGlobal(bytes.Length);

        using var stream = new UnmanagedMemoryStream(
            (byte*) pointer.ToPointer(),
            length: 0,
            bytes.Length,
            FileAccess.Write);

        stream.Write(bytes);

        return pointer;
    }

    internal static (nint Head, nint[] Pointers) WriteUtf8StringArray(string[] values)
    {
        var size = Marshal.SizeOf<nint>();
        var head = Marshal.AllocHGlobal(size * values.Length);
        var pointers = new nint[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            pointers[i] = WriteUtf8String(values[i]);

            Marshal.WriteIntPtr(head + i * size, pointers[i]);
        }

        return (head, pointers);
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

    internal static nint WriteStruct<T>(T value)
    {
        var handle = Marshal.AllocHGlobal(Marshal.SizeOf(value));
        Marshal.StructureToPtr(value, handle, fDeleteOld: false);

        return handle;
    }

    internal static bool TryToWriteStruct<T>(T? value, out nint handle)
    {
        if (value == null)
        {
            handle = nint.Zero;

            return false;
        }

        handle = WriteStruct<T>(value);

        return true;
    }

    internal static void Release(nint pointer)
    {
        Marshal.FreeHGlobal(pointer);
    }

    internal static void ReleaseArray(nint[] pointers)
    {
        foreach (var pointer in pointers)
        {
            Release(pointer);
        }
    }

    internal static bool TryRelease(nint pointer)
    {
        if (pointer == nint.Zero)
        {
            return false;
        }

        Release(pointer);

        return true;
    }
}
