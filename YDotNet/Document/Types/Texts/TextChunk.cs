using System.Runtime.InteropServices;
using YDotNet.Document.Cells;
using YDotNet.Document.Types.Maps;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using YDotNet.Native.Types.Maps;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     Represents a chunk of text formatted with the same set of attributes.
/// </summary>
public class TextChunk
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="TextChunk" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal TextChunk(nint handle)
    {
        Data = new Output(handle, false);

        var offset = Marshal.SizeOf<OutputNative>();
        var attributesLength = (uint) Marshal.ReadInt32(handle + offset);
        var attributesHandle = Marshal.ReadIntPtr(handle + offset + MemoryConstants.PointerSize);

        Attributes = MemoryReader.TryReadIntPtrArray(
                             attributesHandle, attributesLength, Marshal.SizeOf<MapEntryNative>())
                         ?.Select(x => new MapEntry(x))
                         .ToArray() ??
                     Enumerable.Empty<MapEntry>();
    }

    /// <summary>
    ///     Gets the piece of <see cref="Text" /> formatted using the same attributes.
    ///     It can be a string, embedded object or another shared type.
    /// </summary>
    public Output Data { get; }

    /// <summary>
    ///     Gets the formatting attributes applied to the <see cref="Data" />.
    /// </summary>
    public IEnumerable<MapEntry> Attributes { get; }
}
