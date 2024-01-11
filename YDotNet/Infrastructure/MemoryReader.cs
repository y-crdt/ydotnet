using System.Runtime.InteropServices;
using System.Text;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native;
using YDotNet.Native.Types;

namespace YDotNet.Infrastructure;

internal static class MemoryReader
{
    internal static byte[] ReadBytes(nint handle, uint length)
    {
        var data = new byte[length];

        Marshal.Copy(handle, data, startIndex: 0, (int) length);

        return data;
    }

    internal static T[] ReadStructs<T>(nint handle, uint length)
        where T : struct
    {
        var itemSize = Marshal.SizeOf<T>();
        var itemBuffer = new T[length];

        for (var i = 0; i < length; i++)
        {
            itemBuffer[i] = Marshal.PtrToStructure<T>(handle);
            handle += itemSize;
        }

        return itemBuffer;
    }

    internal static nint[] ReadPointers<T>(nint handle, uint length)
        where T : struct
    {
        var itemSize = Marshal.SizeOf<T>();
        var itemBuffer = new nint[length];

        for (var i = 0; i < length; i++)
        {
            itemBuffer[i] = handle;
            handle += itemSize;
        }

        return itemBuffer;
    }

    internal static NativeWithHandle<T>[] ReadStructsWithHandles<T>(nint handle, uint length)
        where T : struct
    {
        var itemSize = Marshal.SizeOf<T>();
        var itemBuffer = new NativeWithHandle<T>[length];

        for (var i = 0; i < length; i++)
        {
            itemBuffer[i] = new NativeWithHandle<T>(Marshal.PtrToStructure<T>(handle), handle);
            handle += itemSize;
        }

        return itemBuffer;
    }

    internal static T ReadStruct<T>(nint handle)
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
