namespace YDotNet.Infrastructure;

public static class Extensions
{
    public static nint Checked(this nint input)
    {
        if (input == nint.Zero)
        {
            ThrowHelper.InternalError();
        }

        return input;
    }
}
