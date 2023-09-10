using YDotNet.Document.Types.Branches;
using YDotNet.Native.StickyIndex;

namespace YDotNet.Document.StickyIndexes;

/// <summary>
///     A sticky index keeps track of the number index of positions inside <see cref="Branch" /> instances
///     even when changes happen to the content.
/// </summary>
/// <remarks>
///     For example, placing sticky index before a certain character will always point to this character.
///     Also, placing a sticky index at the end of a <see cref="Branch" /> will always point to the end of that
///     <see cref="Branch" />.
/// </remarks>
public class StickyIndex : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="StickyIndex" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal StickyIndex(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        StickyIndexChannel.Destroy(Handle);
    }
}
