using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to send information to be stored.
/// </summary>
public class Input
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="doc">The <see cref="Doc" /> instance to be stored in the cell.</param>
    public Input(Doc doc)
    {
        InputNative = InputChannel.Doc(doc.Handle);
    }

    /// <summary>
    ///     Gets the native input cell represented by this cell.
    /// </summary>
    internal InputNative InputNative { get; }
}
