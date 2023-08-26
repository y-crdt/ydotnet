using YDotNet.Document.Types.Branches;
using YDotNet.Infrastructure;
using YDotNet.Native.UndoManager;

namespace YDotNet.Document.UndoManagers;

/// <summary>
///     The <see cref="UndoManager" /> is used to perform undo/redo operations over shared types in a <see cref="Doc" />.
/// </summary>
public class UndoManager : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UndoManager" /> class.
    /// </summary>
    /// <param name="doc">The <see cref="Doc" /> to operate over.</param>
    /// <param name="branch">The shared type in the <see cref="Doc" /> to operate over.</param>
    /// <param name="options">The options to initialize the <see cref="UndoManager" />.</param>
    public UndoManager(Doc doc, Branch branch, UndoManagerOptions? options = null)
    {
        MemoryWriter.TryToWriteStruct(UndoManagerOptionsNative.From(options), out var optionsHandle);

        Handle = UndoManagerChannel.NewWithOptions(doc.Handle, branch.Handle, optionsHandle);

        MemoryWriter.TryRelease(optionsHandle);
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        UndoManagerChannel.Destroy(Handle);
    }
}
