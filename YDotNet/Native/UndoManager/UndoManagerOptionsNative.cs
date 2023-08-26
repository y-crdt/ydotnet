using YDotNet.Document.UndoManagers;

namespace YDotNet.Native.UndoManager;

internal struct UndoManagerOptionsNative
{
    internal uint CaptureTimeoutMilliseconds { get; set; }

    internal static UndoManagerOptionsNative? From(UndoManagerOptions? options)
    {
        if (options == null)
        {
            return null;
        }

        return new UndoManagerOptionsNative
        {
            CaptureTimeoutMilliseconds = options.CaptureTimeoutMilliseconds
        };
    }
}
