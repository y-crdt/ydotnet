namespace YDotNet.Document.Transactions;

/// <summary>
///     Represents the result of applying an update to see <see cref="Doc" /> through a <see cref="Transaction" />.
/// </summary>
/// <remarks>
///     The values match the error codes of the native library.
/// </remarks>
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
    EndOfStream = 3,

    /// <summary>
    ///     Decoded enum tag value was not among known cases.
    /// </summary>
    /// <remarks>
    ///     This also happens when the update contains content that the native library doesn't know, for example array
    ///     moves written by yrs 0.26 or older, which yrs 0.27 removed.
    /// </remarks>
    UnexpectedValue = 4,

    /// <summary>
    ///     Failure when trying to decode JSON content.
    /// </summary>
    InvalidJson = 5,

    /// <summary>
    ///     Other error type than the ones specified.
    /// </summary>
    Other = 6,

    /// <summary>
    ///     Not enough memory to decode the update.
    /// </summary>
    NotEnoughMemory = 7,

    /// <summary>
    ///     A decoded value did not have the expected type.
    /// </summary>
    TypeMismatch = 8,

    /// <summary>
    ///     Custom error reported by the native library.
    /// </summary>
    Custom = 9,
}
