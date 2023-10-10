using System.Runtime.Serialization;

namespace YDotNet;

[Serializable]
public class YDotNetException : Exception
{
    public YDotNetException()
    {
    }

    public YDotNetException(string message)
        : base(message)
    {
    }

    public YDotNetException(string message, Exception inner)
        : base(message, inner)
    {
    }

    protected YDotNetException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
