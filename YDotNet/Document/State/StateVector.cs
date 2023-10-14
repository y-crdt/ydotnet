using YDotNet.Native.Document.State;

namespace YDotNet.Document.State;

/// <summary>
///     Represents the state of a <see cref="Doc" />.
/// </summary>
/// <remarks>
///     It contains the last seen clocks for blocks submitted per any of the clients collaborating on document updates.
/// </remarks>
public class StateVector
{
    internal StateVector(StateVectorNative native)
    {
        var allClientIds = native.ClientIds();
        var allClocks = native.Clocks();

        var state = new Dictionary<ulong, uint>();

        for (var i = 0; i < native.EntriesCount; i++)
        {
            state.Add(allClientIds[i], allClocks[i]);
        }

        State = state;
    }

    /// <summary>
    ///     Gets dictionary of unique client identifiers (keys) by their clocks (values).
    /// </summary>
    public IReadOnlyDictionary<ulong, uint> State { get; }
}
