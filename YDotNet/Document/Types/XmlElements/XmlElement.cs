using System.Runtime.InteropServices;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Branches;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     A shared data type that represents a XML element.
/// </summary>
public class XmlElement : Branch
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="XmlElement" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal XmlElement(nint handle)
        : base(handle)
    {
        // Nothing here.
    }

    /// <summary>
    ///     Gets the name (or tag) of the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     This property returns <c>null</c> for root-level XML nodes.
    /// </remarks>
    public string? Tag
    {
        get
        {
            var handle = XmlElementChannel.Tag(Handle);
            var result = Marshal.PtrToStringAnsi(handle);
            StringChannel.Destroy(handle);

            return result;
        }
    }

    /// <summary>
    ///     Gets the string representation of the <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     The returned value has no padding or indentation spaces.
    /// </remarks>
    public string String
    {
        get
        {
            var handle = XmlElementChannel.String(Handle);
            var result = Marshal.PtrToStringAnsi(handle);
            StringChannel.Destroy(handle);

            return result;
        }
    }

    /// <summary>
    ///     Inserts an attribute in this <see cref="XmlElement" /> instance.
    /// </summary>
    /// <remarks>
    ///     If another attribute with the same <see cref="name" /> already exists, it will be replaced.
    /// </remarks>
    /// <param name="transaction">The transaction that wraps this operation.</param>
    /// <param name="name">The name of the attribute to be added.</param>
    /// <param name="value">The value of the attribute to be added.</param>
    public void InsertAttribute(Transaction transaction, string name, string value)
    {
        XmlElementChannel.InsertAttribute(Handle, transaction.Handle, name, value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return String;
    }
}
