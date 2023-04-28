using System.Runtime.InteropServices;
using System.Text;

namespace YDotNet.Native.Convert;

public static class StringConverter
{
    public static byte[] ToUtf8Bytes(this string value)
    {
        return Encoding.UTF8.GetBytes(value);
    }

    public static string? FromUtf8Bytes(this nint value)
    {
        return Marshal.PtrToStringUTF8(value);
    }
}
