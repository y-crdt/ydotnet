using YDotNet.Document.Cells;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents an entry of a <see cref="Map" />. It contains a <see cref="Key" /> and <see cref="Value" />.
/// </summary>
public sealed class MapEntry
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapEntry" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal MapEntry(nint handle, bool shouldDispose)
    {
        Handle = handle;

        var (mapEntry, outputHandle) = MemoryReader.ReadMapEntryAndOutputHandle(handle);
        Key = MemoryReader.ReadUtf8String(mapEntry.Field);

        // The output memory is part of the entry memory. Therefore we don't release it.
        Value = new Output(outputHandle, false);

        if (shouldDispose)
        {
            // We are done reading and can release the memory.
            MapChannel.EntryDestroy(handle);
        }
    }

    /// <summary>
    ///     Gets the key of this <see cref="MapEntry" /> that represents the <see cref="Value" />.
    /// </summary>
    public string Key { get; }

    /// <summary>
    ///     Gets the value of this <see cref="MapEntry" /> that is represented the <see cref="Key" />.
    /// </summary>
    public Output Value { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
