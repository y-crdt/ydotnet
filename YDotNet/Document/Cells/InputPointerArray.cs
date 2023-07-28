using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents an <see cref="Input" /> cell with a single pointer to be disposed later.
/// </summary>
internal class InputPointerArray : Input
{
    private readonly nint[] pointers;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InputPointerArray" /> class.
    /// </summary>
    /// <param name="inputNative">The native resource that is held by this cell.</param>
    /// <param name="pointers">The array of pointers to be disposed on `Dispose()`.</param>
    internal InputPointerArray(InputNative inputNative, nint[] pointers)
    {
        this.pointers = pointers;

        InputNative = inputNative;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        MemoryWriter.ReleaseArray(pointers);
    }
}
