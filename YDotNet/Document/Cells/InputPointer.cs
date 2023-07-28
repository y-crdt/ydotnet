using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents an <see cref="Input" /> cell with a single pointer to be disposed later.
/// </summary>
internal class InputPointer : Input
{
    private readonly nint pointer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="InputPointer" /> class.
    /// </summary>
    /// <param name="inputNative">The native resource that is held by this cell.</param>
    /// <param name="pointer">The pointer of the unmanaged memory location to be disposed on `Dispose()`.</param>
    internal InputPointer(InputNative inputNative, nint pointer)
    {
        this.pointer = pointer;

        InputNative = inputNative;
    }

    /// <inheritdoc />
    public override void Dispose()
    {
        MemoryWriter.Release(pointer);
    }
}
