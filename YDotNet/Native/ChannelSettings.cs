namespace YDotNet.Native;

internal static class ChannelSettings
{
#if WINDOWS
    public const string NativeLib = "yrs.dll";
#else
    public const string NativeLib = "libyrs.dylib";
#endif
}
