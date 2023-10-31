using YDotNet.Document.Cells;
using YDotNet.Native;
using YDotNet.Native.Types.Texts;

namespace YDotNet.Document.Types.Texts;

/// <summary>
///     Represents a chunk of text formatted with the same set of attributes.
/// </summary>
public class TextChunk
{
    internal TextChunk(NativeWithHandle<TextChunkNative> native, Doc doc)
    {
        // `Handle` is used because the `OutputNative` is located at the head of `TextChunkNative`.
        Data = new Output(native.Handle, doc, isDeleted: false);

        Attributes = native.Value.Attributes()
            .ToDictionary(
                x => x.Value.Key(),
                x => new Output(x.Value.ValueHandle(x.Handle), doc, isDeleted: false));
    }

    /// <summary>
    ///     Gets the piece of <see cref="Text" /> formatted using the same attributes.
    /// </summary>
    /// <remarks>
    ///     It can be a string, embedded object or another shared type.
    /// </remarks>
    public Output Data { get; }

    /// <summary>
    ///     Gets the formatting attributes applied to the <see cref="Data" />.
    /// </summary>
    public IReadOnlyDictionary<string, Output> Attributes { get; }
}
