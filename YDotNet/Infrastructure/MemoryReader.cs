using System.Runtime.InteropServices;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Infrastructure;

internal static class MemoryReader
{
    private static readonly int PointerSize = Marshal.SizeOf<nint>();

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

    internal static nint[] ReadIntPtrArray(nint handle, uint length, int size)
    {
        var result = new nint[length];

        for (var i = 0; i < result.Length; i++)
        {
            var output = handle + i * size;
            result[i] = output;
        }

        return result;
    }

    internal static nint[]? TryReadIntPtrArray(nint handle, uint length, int size)
    {
        return handle == nint.Zero ? null : ReadIntPtrArray(handle, length, size);
    }

    internal static (MapEntryNative MapEntryNative, nint OutputHandle) ReadMapEntryAndOutputHandle(nint handle)
    {
        return (Marshal.PtrToStructure<MapEntryNative>(handle), handle + PointerSize);
    }
}
