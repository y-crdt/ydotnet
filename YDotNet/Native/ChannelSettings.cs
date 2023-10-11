namespace YDotNet.Native;

internal static class ChannelSettings
{
    // The file extension doesn't need to be defined here because the `DllImport` attribute adds it automatically.
    //
    // More information: https://learn.microsoft.com/en-us/dotnet/standard/native-interop/native-library-loading
    public const string NativeLib = "yrs";
}
