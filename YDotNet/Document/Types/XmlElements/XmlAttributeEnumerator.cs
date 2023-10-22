using System.Collections;
using YDotNet.Infrastructure;
using YDotNet.Native.Types;

namespace YDotNet.Document.Types.XmlElements;

/// <summary>
///     Represents the iterator to provide instances of key value paris.
/// </summary>
internal class XmlAttributeEnumerator : IEnumerator<KeyValuePair<string, string>>
{
    private readonly XmlAttributeIterator iterator;

    internal XmlAttributeEnumerator(XmlAttributeIterator iterator)
    {
        this.iterator = iterator;
    }

    /// <inheritdoc />
    public KeyValuePair<string, string> Current { get; private set; }

    /// <inheritdoc />
    object IEnumerator.Current => Current;

    /// <inheritdoc />
    public void Dispose()
    {
        iterator.Dispose();
    }

    /// <inheritdoc />
    public bool MoveNext()
    {
        var handle = XmlAttributeChannel.IteratorNext(iterator.Handle);

        if (handle != nint.Zero)
        {
            var native = MemoryReader.ReadStruct<XmlAttributeNative>(handle);

            Current = new KeyValuePair<string, string>(native.Key(), native.Value());

            // We are done reading, therefore we can release memory.
            XmlAttributeChannel.Destroy(handle);

            return true;
        }

        Current = default;
        return false;
    }

    /// <inheritdoc />
    public void Reset()
    {
        throw new NotImplementedException();
    }
}
