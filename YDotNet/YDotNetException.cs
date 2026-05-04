namespace YDotNet;

/// <summary>
/// Represents an YDotNetException.
/// </summary>
public class YDotNetException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="YDotNetException"/> class.
    /// </summary>
    public YDotNetException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YDotNetException"/> class with error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public YDotNetException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="YDotNetException"/> class with error message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="inner">The inner exception.</param>
    public YDotNetException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
