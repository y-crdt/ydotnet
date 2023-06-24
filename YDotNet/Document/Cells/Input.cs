using System.Runtime.InteropServices;
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
    /// <param name="value">The <see cref="Doc" /> instance to be stored in the cell.</param>
    public Input(Doc value)
    {
        InputNative = InputChannel.Doc(value.Handle);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="string" /> value to be stored in the cell.</param>
    public Input(string value)
    {
        // TODO [LSViana] Free the memory allocated here.
        InputNative = InputChannel.String(Marshal.StringToHGlobalAuto(value));
    }

    /// <summary>
    ///     Gets the native input cell represented by this cell.
    /// </summary>
    internal InputNative InputNative { get; }
}
