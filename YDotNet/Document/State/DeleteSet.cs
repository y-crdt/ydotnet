using YDotNet.Native.Document.State;

namespace YDotNet.Document.State;

/// <summary>
///     Represents the deleted changes in a <see cref="Doc" />.
/// </summary>
public class DeleteSet
{
    internal DeleteSet(DeleteSetNative native)
    {
        var allClients = native.Clients();
        var allRanges = native.Ranges();

        var ranges = new Dictionary<ulong, IdRange[]>();

        for (var i = 0; i < native.EntriesCount; i++)
        {
            ranges.Add(allClients[i], allRanges[i].Sequence().Select(IdRange.Create).ToArray());
        }

        Ranges = ranges;
    }

    /// <summary>
    ///     Gets dictionary of unique client identifiers (keys) by their deleted ID ranges (values).
    /// </summary>
    public IReadOnlyDictionary<ulong, IdRange[]> Ranges { get; }
}
