namespace YDotNet.Document.State;

/// <summary>
///     Represents the state of a <see cref="Doc" />.
/// </summary>
/// <remarks>
///     It contains the last seen clocks for blocks submitted per any of the clients collaborating on document updates.
/// </remarks>
public class StateVector
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StateVector" /> class.
    /// </summary>
    /// <param name="state">The initial value for the <see cref="State" />.</param>
    public StateVector(Dictionary<ulong, uint> state)
    {
        State = state;
    }

    /// <summary>
    ///     Gets dictionary of unique client identifiers (keys) by their clocks (values).
    /// </summary>
    public Dictionary<ulong, uint> State { get; }
}
