using System.Runtime.InteropServices;
using System.Text;
using YDotNet.Native;
using YDotNet.Native.Types;

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

    internal static IEnumerable<nint> ReadIntPtrArray(nint handle, uint length, int size)
    {
        var itemSize = size;

        for (var i = 0; i < length; i++)
        {
            yield return handle;

            handle += itemSize;
        }
    }

    internal static IEnumerable<NativeWithHandle<T>> ReadIntPtrArray<T>(nint handle, uint length) where T : struct
    {
        var itemSize = Marshal.SizeOf<T>();

        for (var i = 0; i < length; i++)
        {
            yield return new NativeWithHandle<T>(Marshal.PtrToStructure<T>(handle), handle);

            handle += itemSize;
        }
    }

    internal static nint[]? TryReadIntPtrArray(nint handle, uint length, int size)
    {
        return handle == nint.Zero ? null : ReadIntPtrArray(handle, length, size).ToArray();
    }

    internal static T PtrToStruct<T>(nint handle)
        where T : struct
    {
        return Marshal.PtrToStructure<T>(handle.Checked());
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
