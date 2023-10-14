using System.Collections;
using YDotNet.Document.Cells;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements.Trees;

/// <summary>
///     Represents the tree walker to provide instances of <see cref="Output" /> or
///     <c>null</c> to <see cref="XmlTreeWalker" />.
/// </summary>
internal class XmlTreeWalkerEnumerator : IEnumerator<Output>
{
    private Output? current;

    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlTreeWalkerEnumerator" /> class.
    /// </summary>
    /// <param name="treeWalker">
    ///     The <see cref="TreeWalker" /> instance used by this enumerator.
    ///     Check <see cref="TreeWalker" /> for more details.
    /// </param>
    public XmlTreeWalkerEnumerator(XmlTreeWalker treeWalker)
    {
        TreeWalker = treeWalker;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        TreeWalker.Dispose();
    }

    /// <inheritdoc />
    public Output Current => current!;

    /// <inheritdoc />
    object? IEnumerator.Current => current!;

    private XmlTreeWalker TreeWalker { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = XmlElementChannel.TreeWalkerNext(TreeWalker.Handle);

        if (handle != nint.Zero)
        {
            current = Output.CreateAndRelease(handle, TreeWalker.Doc);
            return true;
        }
        else
        {
            current = null!;
            return false;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
