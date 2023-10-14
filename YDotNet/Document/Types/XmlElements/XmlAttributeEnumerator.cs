using System.Collections;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents the iterator to provide instances of key value paris.
/// </summary>
internal class XmlAttributeEnumerator : IEnumerator<KeyValuePair<string, string>>
{
    private KeyValuePair<string, string> current;

    internal XmlAttributeEnumerator(XmlAttributeIterator iterator)
    {
        Iterator = iterator;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Iterator.Dispose();
    }

    /// <inheritdoc />
    public KeyValuePair<string, string> Current => current;

    /// <inheritdoc />
    object? IEnumerator.Current => current!;

    private XmlAttributeIterator Iterator { get; }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = XmlAttributeChannel.IteratorNext(Iterator.Handle);

        if (handle != nint.Zero)
        {
            var native = MemoryReader.ReadStruct<XmlAttributeNative>(handle);

            current = new KeyValuePair<string, string>(native.Key(), native.Value());

            // We are done reading, therefore we can release memory.
            XmlAttributeChannel.Destroy(handle);

            return true;
        }
        else
        {
            current = default;
            return false;
        }
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
