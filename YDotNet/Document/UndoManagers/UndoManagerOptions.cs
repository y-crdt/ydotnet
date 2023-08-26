namespace YDotNet.Document.UndoManagers;

/// <summary>
///     Represents the set of options used to configure <see cref="UndoManager" />.
/// </summary>
public class UndoManagerOptions
{
    /// <summary>
    ///     Gets the time interval used to capture snapshots.
    /// </summary>
    /// <remarks>
    ///     The updates are grouped together in time-constrained snapshots.
    /// </remarks>
    public uint CaptureTimeoutMilliseconds { get; init; }
}
