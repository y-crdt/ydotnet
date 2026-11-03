using YDotNet.Native.Document.State;

namespace YDotNet.Document.State;

/// <summary>
///     Represents an update that could not be integrated into a <see cref="Doc" /> yet, because other updates it
///     depends on have not been received.
/// </summary>
public class PendingUpdate
{
    internal PendingUpdate(PendingUpdateNative native)
    {
        Missing = new StateVector(native.Missing);
        Update = native.Update();
    }

    /// <summary>
    ///     Gets the state vector with the minimal client clock values that need to be satisfied in order to
    ///     successfully apply <see cref="Update" />.
    /// </summary>
    public StateVector Missing { get; }

    /// <summary>
    ///     Gets the pending update, encoded in the lib0 v1 format.
    /// </summary>
    public byte[] Update { get; }
}
