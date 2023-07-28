using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents an <see cref="Input" /> cell with no resources to be disposed later.
/// </summary>
internal class InputEmpty : Input
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="InputEmpty" /> class.
    /// </summary>
    /// <param name="inputNative">The native resource that is held by this cell.</param>
    internal InputEmpty(InputNative inputNative)
    {
        InputNative = inputNative;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        // Nothing here.
    }
}
