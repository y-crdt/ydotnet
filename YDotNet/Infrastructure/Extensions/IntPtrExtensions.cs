namespace YDotNet.Infrastructure.Extensions;

internal static class IntPtrExtensions
{
    public static nint Checked(this nint input)
    {
        if (input == nint.Zero)
        {
            ThrowHelper.Null();
        }

        return input;
    }
}
