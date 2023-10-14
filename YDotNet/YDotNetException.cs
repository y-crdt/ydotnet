using System.Runtime.Serialization;

namespace YDotNet;

/// <summary>
/// Represents an YDotNetException.
/// </summary>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="YDotNetException"/> class for serialization.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The serialization context.</param>
    protected YDotNetException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
