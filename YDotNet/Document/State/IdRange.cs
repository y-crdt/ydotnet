namespace YDotNet.Document.State;

/// <summary>
///     Represents a single space of clock values, belonging to the same client.
/// </summary>
public class IdRange
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IdRange" /> class.
    /// </summary>
    /// <param name="start">The value for <see cref="Start" />.</param>
    /// <param name="end">The value for <see cref="End" />.</param>
    public IdRange(uint start, uint end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    ///     Gets the start of the <see cref="IdRange" />.
    /// </summary>
    public uint Start { get; }

    /// <summary>
    ///     Gets the end of the <see cref="IdRange" />.
    /// </summary>
    public uint End { get; }
}
