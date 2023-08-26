using YDotNet.Document.Events;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.UndoManagers.Events;
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

    /// <summary>
    ///     Subscribes a callback function to be called every time a new an update happens in a tracked shared type
    ///     after the capture timeout from the previous update has been reached or <see cref="Reset" /> has been called.
    ///     has been called.
    /// </summary>
    /// <param name="action">The callback to be executed when an update happens, respecting the capture timeout.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription ObserveAdded(Action<UndoEvent> action)
    {
        var subscriptionId = UndoManagerChannel.ObserveAdded(
            Handle,
            nint.Zero,
            (state, undoEvent) => action(undoEvent.ToUndoEvent()));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, previously
    ///     registered via <see cref="ObserveAdded" />.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveAfterTransaction(EventSubscription subscription)
    {
        UndoManagerChannel.UnobserveAdded(Handle, subscription.Id);
    }
}
