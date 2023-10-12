using System.Runtime.InteropServices;
using YDotNet.Document;
using YDotNet.Document.Events;

namespace YDotNet.Native.Document.Events;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct ClearEventNative
{
    public Doc Doc { get; init; }

    public static ClearEventNative From(Doc doc)
    {
        return new ClearEventNative { Doc = doc };
    }

    public ClearEvent ToClearEvent()
    {
        return new ClearEvent(Doc);
    }
}
