using YDotNet.Document.UndoManagers;

namespace YDotNet.Native.UndoManager;

internal struct UndoManagerOptionsNative
{
    internal uint CaptureTimeoutMilliseconds { get; set; }

    internal static UndoManagerOptionsNative From(UndoManagerOptions? options)
    {
        return new UndoManagerOptionsNative
        {
            CaptureTimeoutMilliseconds = options?.CaptureTimeoutMilliseconds ?? 0
        };
    }
}
