using YDotNet.Document.Cells;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents an entry of a <see cref="Map" />.
/// </summary>
public sealed class MapEntry : UnmanagedResource
{
    internal MapEntry(nint handle, Doc doc, IResourceOwner? owner)
        : base(handle)
    {
        var (mapEntry, outputHandle) = MemoryReader.ReadMapEntryAndOutputHandle(handle);
        Key = MemoryReader.ReadUtf8String(mapEntry.Field);

        // The output can not released independently and will be released when the entry is not needed anymore.
        Value = new Output(outputHandle, doc, owner ?? this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MapEntry"/> class.
    /// </summary>
    ~MapEntry()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        MapChannel.EntryDestroy(Handle);
    }

    /// <summary>
    ///     Gets the key of this <see cref="MapEntry" /> that represents the <see cref="Value" />.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the value of this <see cref="MapEntry" /> that is represented the <see cref="Key" />.
    /// </summary>
    public Output Value { get; }
}
