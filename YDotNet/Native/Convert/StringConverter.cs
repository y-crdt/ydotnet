using System.Text;

namespace YDotNet.Native.Convert;

public static class StringConverter
{
    public static byte[] ToBytes(this string? value)
    {
        if (value is null)
        {
            return Array.Empty<byte>();
        }

        return Encoding.UTF8.GetBytes(value);
    }
}
