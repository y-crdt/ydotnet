using System.Transactions;
using YDotNet.Document.Events;
using YDotNet.Document.Types.Branches;
using YDotNet.Document.UndoManagers.Events;
using YDotNet.Infrastructure;
using YDotNet.Native.UndoManager;

namespace YDotNet.Document.UndoManagers;

/// <summary>
///     The <see cref="UndoManager" /> is used to perform undo/redo operations over shared types in a <see cref="Doc" />.
/// </summary>
public class UndoManager : UnmanagedResource
{
    private readonly EventSubscriber<UndoEvent> onAdded;
    private readonly EventSubscriber<UndoEvent> onPopped;

    /// <summary>
    ///     Initializes a new instance of the <see cref="UndoManager" /> class.
    /// </summary>
    /// <param name="doc">The <see cref="Doc" /> to operate over.</param>
    /// <param name="branch">The shared type in the <see cref="Doc" /> to operate over.</param>
    /// <param name="options">The options to initialize the <see cref="UndoManager" />.</param>
    public UndoManager(Doc doc, Branch branch, UndoManagerOptions? options = null)
        : base(Create(doc, branch, options))
    {
        onAdded = new EventSubscriber<UndoEvent>(
            doc.EventManager,
            Handle,
            (_, action) =>
            {
                UndoManagerChannel.ObserveAddedCallback callback =
                    (_, undoEvent) => action(new UndoEvent(undoEvent));

                return (UndoManagerChannel.ObserveAdded(Handle, nint.Zero, callback), callback);
            },
            (owner, s) => UndoManagerChannel.UnobserveAdded(owner, s));

        onPopped = new EventSubscriber<UndoEvent>(
            doc.EventManager,
            Handle,
            (_, action) =>
            {
                UndoManagerChannel.ObservePoppedCallback callback =
                    (_, undoEvent) => action(new UndoEvent(undoEvent));

                return (UndoManagerChannel.ObservePopped(Handle, nint.Zero, callback), callback);
            },
            (owner, s) => UndoManagerChannel.UnobservePopped(owner, s));
    }

    /// <summary>
    ///     Finalizes an instance of the <see cref="UndoManager" /> class.
    /// </summary>
    ~UndoManager()
    {
        Dispose(disposing: false);
    }

    /// <summary>
    ///     Subscribes a callback function to be called every time a new an update happens in a tracked shared type
    ///     after the capture timeout from the previous update has been reached or <see cref="Stop" /> has been called.
    ///     has been called.
    /// </summary>
    /// <param name="action">The callback to be executed when an update happens, respecting the capture timeout.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveAdded(Action<UndoEvent> action)
    {
        return onAdded.Subscribe(action);
    }

    /// <summary>
    ///     Subscribes a callback function to be called every time <see cref="Undo" /> or <see cref="Redo" /> is executed
    ///     and removes items from the history of changes.
    /// </summary>
    /// <param name="action">The callback to be executed when <see cref="Undo" /> or <see cref="Redo" /> is executed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObservePopped(Action<UndoEvent> action)
    {
        return onPopped.Subscribe(action);
    }

    /// <summary>
    ///     Undoes the last changes tracked by the <see cref="UndoManager" />.
    /// </summary>
    /// <remarks>
    ///     The group of actions to be undone corresponds to the group of actions that happened within
    ///     the capture timeout or since <see cref="Stop" /> was called.
    /// </remarks>
    /// <returns>A boolean flag indicating whether the undo operation performed any changes.</returns>
    public bool Undo()
    {
        return UndoManagerChannel.Undo(Handle) == 1;
    }

    /// <summary>
    ///     Redoes the last changes tracked by the <see cref="UndoManager" />.
    /// </summary>
    /// <remarks>
    ///     The group of actions to be redone corresponds to the group of actions that happened within
    ///     the capture timeout or since <see cref="Stop" /> was called.
    /// </remarks>
    /// <returns>A boolean flag indicating whether the redo operation performed any changes.</returns>
    public bool Redo()
    {
        return UndoManagerChannel.Redo(Handle) == 1;
    }

    /// <summary>
    ///     Returns a <see cref="bool" /> indicating whether it's possible to undo changes.
    /// </summary>
    /// <returns><see cref="bool" /> indicating whether it's possible to undo changes.</returns>
    public bool CanUndo()
    {
        return UndoManagerChannel.CanUndo(Handle) == 1;
    }

    /// <summary>
    ///     Returns a <see cref="bool" /> indicating whether it's possible to redo changes.
    /// </summary>
    /// <returns><see cref="bool" /> indicating whether it's possible to redo changes.</returns>
    public bool CanRedo()
    {
        return UndoManagerChannel.CanRedo(Handle) == 1;
    }

    /// <summary>
    ///     Resets the <see cref="UndoManager" /> and removes history to undo/redo changes.
    /// </summary>
    /// <returns><see cref="bool" /> indicating whether the state was reset.</returns>
    public bool Clear()
    {
        return UndoManagerChannel.Clear(Handle) == 1;
    }

    /// <summary>
    ///     Stops the current capturing of changes and starts a new capturing group.
    /// </summary>
    public void Stop()
    {
        UndoManagerChannel.Stop(Handle);
    }

    /// <summary>
    ///     Includes the <see cref="Branch" /> instance in the list of shared types tracked by this <see cref="UndoManager" />.
    /// </summary>
    /// <param name="branch">The <see cref="Branch" /> to be tracked by this <see cref="UndoManager" />.</param>
    public void AddScope(Branch branch)
    {
        UndoManagerChannel.AddScope(Handle, branch.Handle);
    }

    /// <summary>
    ///     Includes this origin in the list of tracked origins for this <see cref="UndoManager" />.
    /// </summary>
    /// <remarks>
    ///     Origin can be assigned to updates executing in a scope of a particular <see cref="Transaction" />
    ///     via the parameters of <see cref="Doc.WriteTransaction" />.
    /// </remarks>
    /// <param name="origin">The origin to be included in this <see cref="UndoManager" />.</param>
    public void AddOrigin(byte[] origin)
    {
        UndoManagerChannel.AddOrigin(Handle, (uint) origin.Length, origin);
    }

    /// <summary>
    ///     Removes this origin from the list of tracked origins for this <see cref="UndoManager" />.
    /// </summary>
    /// <remarks>
    ///     Origin can be assigned to updates executing in a scope of a particular <see cref="Transaction" />
    ///     via the parameters of <see cref="Doc.WriteTransaction" />.
    /// </remarks>
    /// <param name="origin">The origin to be removed from this <see cref="UndoManager" />.</param>
    public void RemoveOrigin(byte[] origin)
    {
        UndoManagerChannel.RemoveOrigin(Handle, (uint) origin.Length, origin);
    }

    /// <inheritdoc />
    protected internal override void DisposeCore(bool disposing)
    {
        UndoManagerChannel.Destroy(Handle);
    }

    private static nint Create(Doc doc, Branch branch, UndoManagerOptions? options)
    {
        var unsafeOptions = MemoryWriter.WriteStruct(options?.ToNative() ?? default);

        return UndoManagerChannel.NewWithOptions(doc.Handle, branch.Handle, unsafeOptions.Handle);
    }
}
