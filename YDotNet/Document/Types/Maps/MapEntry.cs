using YDotNet.Document.Cells;
using YDotNet.Infrastructure;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Maps;

/// <summary>
///     Represents an entry of a <see cref="Map" />. It contains a <see cref="Key" /> and <see cref="Value" />.
/// </summary>
public class MapEntry : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapEntry" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal MapEntry(nint handle)
    {
        Handle = handle;

        var (mapEntry, outputHandle) = MemoryReader.ReadMapEntryAndOutputHandle(handle);

        MapEntryNative = mapEntry;
        Value = new Output(outputHandle);
    }

    /// <summary>
    ///     Gets the key of this <see cref="MapEntry" /> that represents the <see cref="Value" />.
    /// </summary>
    public string Key => MapEntryNative.Field;

    /// <summary>
    ///     Gets the value of this <see cref="MapEntry" /> that is represented the <see cref="Key" />.
    /// </summary>
    public Output Value { get; }

    /// <summary>
    ///     Gets the native resource that provides data for this <see cref="MapEntry" /> instance.
    /// </summary>
    internal MapEntryNative MapEntryNative { get; }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        MapChannel.EntryDestroy(Handle);
    }
}
