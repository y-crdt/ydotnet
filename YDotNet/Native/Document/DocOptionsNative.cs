using System.Runtime.InteropServices;

namespace YDotNet.Native.Document;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DocOptionsNative
{
    public ulong Id { get; init; }

    public nint Guid { get; init; }

    public nint CollectionId { get; init; }

    public byte Encoding { get; init; }

    public byte SkipGc { get; init; }

    public byte AutoLoad { get; init; }

    public byte ShouldLoad { get; init; }
}
