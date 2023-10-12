using System.Runtime.InteropServices;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using Array = YDotNet.Document.Types.Arrays.Array;

#pragma warning disable SA1623 // Property summary documentation should match accessors

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to read information from the storage.
/// </summary>
public sealed class Output
{
    private readonly object? value;

    internal Output(nint handle, Doc doc)
    {
        var native = MemoryReader.PtrToStruct<OutputNative>(handle);

        Type = (OutputType)native.Tag;

        value = BuildValue(handle, native.Length, doc, Type);
    }

    internal static Output CreateAndRelease(nint handle, Doc doc)
    {
        var result = new Output(handle, doc);

        // The output reads everything so we can just destroy it.
        OutputChannel.Destroy(handle);

        return result;
    }

    /// <summary>
    ///     Gets the type of the output.
    /// </summary>
    public OutputType Type { get; private set; }

    /// <summary>
    ///     Gets the <see cref="Doc" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public Doc Doc => GetValue<Doc>(OutputType.Doc);

    /// <summary>
    ///     Gets the <see cref="string" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="string" />.</exception>
    public string String => GetValue<string>(OutputType.String);

    /// <summary>
    ///     Gets the <see cref="bool" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="string" />.</exception>
    public bool Boolean => GetValue<bool>(OutputType.Bool);

    /// <summary>
    ///     Gets the <see cref="double" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="double" />.</exception>
    public double Double => GetValue<double>(OutputType.Double);

    /// <summary>
    ///     Gets the <see cref="long" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="long" />.</exception>
    public long Long => GetValue<long>(OutputType.Long);

    /// <summary>
    ///     Gets the <see cref="byte" /> array value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="byte" /> array.</exception>
    public byte[] Bytes => GetValue<byte[]>(OutputType.Bytes);

    /// <summary>
    ///     Gets the <see cref="Output" /> collection.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Output" /> collection.</exception>
    public JsonArray Collection => GetValue<JsonArray>(OutputType.Collection);

    /// <summary>
    ///     Gets the value as json object.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a json object.</exception>
    public JsonObject Object => GetValue<JsonObject>(OutputType.Object);

    /// <summary>
    ///     Gets the <see cref="Array" /> value.
    /// </summary>
    /// <returns>The resolved array.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Array" />.</exception>
    public Array Array => GetValue<Array>(OutputType.Array);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved map.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    public Map Map => GetValue<Map>(OutputType.Map);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    public Text Text => GetValue<Text>(OutputType.Text);

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> value.
    /// </summary>
    /// <returns>The resolved xml element.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlElement" />.</exception>
    public XmlElement XmlElement => GetValue<XmlElement>(OutputType.XmlElement);

    /// <summary>
    ///     Gets the <see cref="XmlText" /> value.
    /// </summary>
    /// <returns>The resolved xml text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlText" />.</exception>
    public XmlText XmlText => GetValue<XmlText>(OutputType.XmlText);

    private static object? BuildValue(nint handle, uint length, Doc doc, OutputType type)
    {
        switch (type)
        {
            case OutputType.Bool:
                {
                    var value = OutputChannel.Boolean(handle).Checked();

                    return Marshal.PtrToStructure<byte>(value) == 1;
                }

            case OutputType.Double:
                {
                    var value = OutputChannel.Double(handle).Checked();

                    return Marshal.PtrToStructure<double>(value);
                }

            case OutputType.Long:
                {
                    var value = OutputChannel.Long(handle).Checked();

                    return Marshal.PtrToStructure<long>(value);
                }

            case OutputType.String:
                {
                    MemoryReader.TryReadUtf8String(OutputChannel.String(handle), out var result);

                    return result;
                }

            case OutputType.Bytes:
                {
                    var bytesHandle = OutputChannel.Bytes(handle).Checked();
                    var bytesArray = MemoryReader.ReadBytes(OutputChannel.Bytes(handle), length);

                    return bytesArray;
                }

            case OutputType.Collection:
                {
                    return new JsonArray(handle, length, doc);
                }

            case OutputType.Object:
                {
                    return new JsonObject(handle, length, doc);
                }

            case OutputType.Array:
                return doc.GetArray(OutputChannel.Array(handle));

            case OutputType.Map:
                return doc.GetMap(OutputChannel.Map(handle));

            case OutputType.Text:
                return doc.GetText(OutputChannel.Text(handle));

            case OutputType.XmlElement:
                return doc.GetXmlElement(OutputChannel.XmlElement(handle));

            case OutputType.XmlText:
                return doc.GetXmlText(OutputChannel.XmlText(handle));

            case OutputType.Doc:
                return doc.GetDoc(OutputChannel.Doc(handle));

            default:
                return null;
        }
    }

    private T GetValue<T>(OutputType expectedType)
    {
        if (value is not T typed)
        {
            throw new YDotNetException($"Expected {expectedType}, got {Type}.");
        }

        return typed;
    }
}
