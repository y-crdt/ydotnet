using YDotNet.Native.Types;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents an array.
/// </summary>
public class Array
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Array" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Array(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the number of elements stored within current instance of <see cref="Types.Array" />.
    /// </summary>
    public uint Length => ArrayChannel.Length(Handle);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }
}
