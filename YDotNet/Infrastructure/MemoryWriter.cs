using System.Runtime.InteropServices;
using System.Text;

namespace YDotNet.Infrastructure;

internal static class MemoryWriter
{
    internal static DisposableHandle WriteUtf8String(string? value)
    {
        if (value == null)
        {
            return new DisposableHandle(Handle: 0);
        }

        return new DisposableHandle(WriteUtf8StringCore(value));
    }

    private static unsafe nint WriteUtf8StringCore(string value)
    {
        var bufferLength = Encoding.UTF8.GetByteCount(value) + 1;
        var bufferPointer = Marshal.AllocHGlobal(bufferLength);

        var memory = new Span<byte>(bufferPointer.ToPointer(), bufferLength);

        Encoding.UTF8.GetBytes(value, memory);
        memory[bufferLength - 1] = (byte)'\0';

        return bufferPointer;
    }

    internal static DisposableHandleArray WriteUtf8StringArray(string[] values)
    {
        var head = Marshal.AllocHGlobal(MemoryConstants.PointerSize * values.Length);
        var pointers = new nint[values.Length];

        for (var i = 0; i < values.Length; i++)
        {
            pointers[i] = WriteUtf8StringCore(values[i]);

            Marshal.WriteIntPtr(head + i * MemoryConstants.PointerSize, pointers[i]);
        }

        return new DisposableHandleArray(head, pointers);
    }

    internal static DisposableHandle WriteStructArray<T>(T[] value)
        where T : struct
    {
        var itemSize = Marshal.SizeOf<T>();
        var itemBuffer = Marshal.AllocHGlobal(itemSize * value.Length);

        for (var i = 0; i < value.Length; i++)
        {
            Marshal.StructureToPtr(value[i], itemBuffer + i * itemSize, fDeleteOld: false);
        }

        return new DisposableHandle(itemBuffer);
    }

    internal static DisposableHandle WriteStruct<T>(T? value)
        where T : struct
    {
        if (value == null)
        {
            return new DisposableHandle(Handle: 0);
        }

        return WriteStruct(value.Value);
    }

    internal static DisposableHandle WriteStruct<T>(T value)
        where T : struct
    {
        var handle = Marshal.AllocHGlobal(Marshal.SizeOf(value));

        Marshal.StructureToPtr(value, handle, fDeleteOld: false);

        return new DisposableHandle(handle);
    }

    internal sealed record DisposableHandle(nint Handle) : IDisposable
    {
        public void Dispose()
        {
            if (Handle != nint.Zero)
            {
                Marshal.FreeHGlobal(Handle);
            }
        }
    }

    internal sealed record DisposableHandleArray(nint Head, nint[] Handles) : IDisposable
    {
        public void Dispose()
        {
            if (Head != nint.Zero)
            {
                Marshal.FreeHGlobal(Head);
            }

            foreach (var handle in Handles)
            {
                if (handle != nint.Zero)
                {
                    Marshal.FreeHGlobal(handle);
                }
            }
        }
    }
}
