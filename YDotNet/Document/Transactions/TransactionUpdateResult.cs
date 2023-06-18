namespace YDotNet.Document.Transactions;

/// <summary>
///     Represents the result of applying an update to see <see cref="Doc" /> through a <see cref="Transaction" />.
/// </summary>
public enum TransactionUpdateResult
{
    /// <summary>
    ///     The update operation succeeded.
    /// </summary>
    Ok = 0,

    /// <summary>
    ///     Couldn't read data from input stream.
    /// </summary>
    Io = 1,

    /// <summary>
    ///     Decoded variable integer outside of the expected integer size bounds.
    /// </summary>
    IntegerOutOfBounds = 2,

    /// <summary>
    ///     End of stream found when more data was expected.
    /// </summary>
    UnexpectedValue = 3,

    /// <summary>
    ///     Decoded enum tag value was not among known cases.
    /// </summary>
    InvalidJson = 4,

    /// <summary>
    ///     Failure when trying to decode JSON content.
    /// </summary>
    Other = 5
}
