using YDotNet.Document.Types.Branches;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types;

/// <summary>
///     A shared data type that represents an array.
/// </summary>
public class Array : Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Array" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Array(nint handle)
        : base(handle)
    {
        // Nothing here.
    }

    /// <summary>
    ///     Gets the number of elements stored within current instance of <see cref="Types.Array" />.
    /// </summary>
    public uint Length => ArrayChannel.Length(Handle);
}
