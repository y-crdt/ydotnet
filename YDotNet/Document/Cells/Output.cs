using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to read information from the storage.
/// </summary>
public sealed class Output
{
    private readonly object? value;

    internal Output(nint handle, Doc doc, bool isDeleted)
    {
        var native = MemoryReader.ReadStruct<OutputNative>(handle);

        Tag = (OutputTag) native.Tag;

        value = BuildValue(handle, native.Length, doc, isDeleted, Tag);
    }

    /// <summary>
    ///     Gets the type of the output.
    /// </summary>
    public OutputTag Tag { get; }

    /// <summary>
    ///     Gets the <see cref="Doc" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public Doc Doc => GetValue<Doc>(OutputTag.Doc);

    /// <summary>
    ///     Gets the <see cref="string" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="string" />.</exception>
    public string String => GetValue<string>(OutputTag.String);

#pragma warning disable SA1623 (This property documentation shouldn't start with the standard text)
    /// <summary>
    ///     Gets the <see cref="bool" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="string" />.</exception>
    public bool Boolean => GetValue<bool>(OutputTag.Bool);
#pragma warning restore SA1623

    /// <summary>
    ///     Gets the <see cref="double" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="double" />.</exception>
    public double Double => GetValue<double>(OutputTag.Double);

    /// <summary>
    ///     Gets the <see cref="long" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="long" />.</exception>
    public long Long => GetValue<long>(OutputTag.Long);

    /// <summary>
    ///     Gets the <see cref="byte" /> array value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="byte" /> array.</exception>
    public byte[] Bytes => GetValue<byte[]>(OutputTag.Bytes);

    /// <summary>
    ///     Gets the <see cref="Output" /> collection.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Output" /> collection.</exception>
    public JsonArray JsonArray => GetValue<JsonArray>(OutputTag.JsonArray);

    /// <summary>
    ///     Gets the value as json object.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a json object.</exception>
    public JsonObject JsonObject => GetValue<JsonObject>(OutputTag.JsonObject);

    /// <summary>
    ///     Gets the <see cref="Array" /> value.
    /// </summary>
    /// <returns>The resolved array.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Array" />.</exception>
    public Array Array => GetValue<Array>(OutputTag.Array);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved map.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    public Map Map => GetValue<Map>(OutputTag.Map);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    public Text Text => GetValue<Text>(OutputTag.Text);

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> value.
    /// </summary>
    /// <returns>The resolved xml element.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlElement" />.</exception>
    public XmlElement XmlElement => GetValue<XmlElement>(OutputTag.XmlElement);

    /// <summary>
    ///     Gets the <see cref="XmlText" /> value.
    /// </summary>
    /// <returns>The resolved xml text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlText" />.</exception>
    public XmlText XmlText => GetValue<XmlText>(OutputTag.XmlText);

    internal static Output CreateAndRelease(nint handle, Doc doc)
    {
        var result = new Output(handle, doc, isDeleted: false);

        // The output reads everything so we can just destroy it.
        OutputChannel.Destroy(handle);

        return result;
    }

    private static object? BuildValue(nint handle, uint length, Doc doc, bool isDeleted, OutputTag type)
    {
        switch (type)
        {
            case OutputTag.Bool:
            {
                var value = OutputChannel.Boolean(handle).Checked();

                return MemoryReader.ReadStruct<byte>(value) == 1;
            }

            case OutputTag.Double:
            {
                var value = OutputChannel.Double(handle).Checked();

                return MemoryReader.ReadStruct<double>(value);
            }

            case OutputTag.Long:
            {
                var value = OutputChannel.Long(handle).Checked();

                return MemoryReader.ReadStruct<long>(value);
            }

            case OutputTag.String:
            {
                MemoryReader.TryReadUtf8String(OutputChannel.String(handle), out var result);

                return result;
            }

            case OutputTag.Bytes:
            {
                var bytesHandle = OutputChannel.Bytes(handle).Checked();
                var bytesArray = MemoryReader.ReadBytes(OutputChannel.Bytes(handle), length);

                return bytesArray;
            }

            case OutputTag.JsonArray:
            {
                return new JsonArray(handle, length, doc, isDeleted);
            }

            case OutputTag.JsonObject:
            {
                return new JsonObject(handle, length, doc, isDeleted);
            }

            case OutputTag.Array:
                return doc.GetArray(OutputChannel.Array(handle), isDeleted);

            case OutputTag.Map:
                return doc.GetMap(OutputChannel.Map(handle), isDeleted);

            case OutputTag.Text:
                return doc.GetText(OutputChannel.Text(handle), isDeleted);

            case OutputTag.XmlElement:
                return doc.GetXmlElement(OutputChannel.XmlElement(handle), isDeleted);

            case OutputTag.XmlText:
                return doc.GetXmlText(OutputChannel.XmlText(handle), isDeleted);

            case OutputTag.Doc:
                return doc.GetDoc(OutputChannel.Doc(handle), isDeleted);

            default:
                return null;
        }
    }

    private T GetValue<T>(OutputTag expectedType)
    {
        if (value is not T typed)
        {
            throw new YDotNetException($"Expected {expectedType}, got {Tag}.");
        }

        return typed;
    }
}
