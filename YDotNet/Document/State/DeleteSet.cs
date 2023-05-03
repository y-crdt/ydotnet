namespace YDotNet.Document.State;

/// <summary>
///     Represents the deleted changes in a <see cref="Doc" />.
/// </summary>
public class DeleteSet
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DeleteSet" /> class.
    /// </summary>
    /// <param name="ranges">The initial value for <see cref="Ranges" />.</param>
    public DeleteSet(Dictionary<ulong, IdRange[]> ranges)
    {
        Ranges = ranges;
    }

    /// <summary>
    ///     Gets dictionary of unique client identifiers (keys) by their deleted ID ranges (values).
    /// </summary>
    public Dictionary<ulong, IdRange[]> Ranges { get; }
}
