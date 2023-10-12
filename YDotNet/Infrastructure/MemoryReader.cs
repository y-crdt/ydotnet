using System.Runtime.InteropServices;
using System.Text;
using YDotNet.Document.State;
using YDotNet.Native.Document.State;
using YDotNet.Native.Types;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Infrastructure;

internal static class MemoryReader
{
    internal static unsafe byte[] ReadBytes(nint handle, uint length)
    {
        var data = new byte[length];

        Marshal.Copy(handle, data, 0, (int)length);

        return data;
    }

    internal static T ReadStruct<T>(nint handle)
        where T : struct
    {
        return Marshal.PtrToStructure<T>(handle);
    }

    internal static T[] ReadStructArray<T>(nint handle, uint length)
        where T : struct
    {
        var itemSize = Marshal.SizeOf<T>();
        var itemBuffer = new T[length];

        for (var i = 0; i < length; i++)
        {
            itemBuffer[i] = Marshal.PtrToStructure<T>(handle + (i * itemSize));
        }

        return itemBuffer;
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
        return (Marshal.PtrToStructure<MapEntryNative>(handle), handle + MemoryConstants.PointerSize);
    }

    internal static string ReadUtf8String(nint handle)
    {
        ReadOnlySpan<byte> readOnlySpan;
        unsafe
        {
            var index = 0;

            while (true)
            {
                if (Marshal.ReadByte(handle + index++) == 0)
                {
                    // Decrease the index to discard the zero byte.
                    index--;
                    break;
                }
            }

            readOnlySpan = new ReadOnlySpan<byte>(handle.ToPointer(), index);
        }

        return Encoding.UTF8.GetString(readOnlySpan);
    }

    internal static bool TryReadUtf8String(nint handle, out string? result)
    {
        if (handle == nint.Zero)
        {
            result = null;
            return false;
        }

        result = ReadUtf8String(handle);

        return true;
    }

    public static byte[] ReadAndDestroyBytes(nint handle, uint length)
    {
        var data = ReadBytes(handle, length);

        BinaryChannel.Destroy(handle, length);
        return data;
    }

    public static string ReadStringAndDestroy(nint handle)
    {
        var result = ReadUtf8String(handle);

        StringChannel.Destroy(handle);
        return result;
    }
}
