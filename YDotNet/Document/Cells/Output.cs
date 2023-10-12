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

        Tag = (OutputTage)native.Tag;

        value = BuildValue(handle, native.Length, doc, Tag);
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
    public OutputTage Tag { get; private set; }

    /// <summary>
    ///     Gets the <see cref="Doc" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Doc" />.</exception>
    public Doc Doc => GetValue<Doc>(OutputTage.Doc);

    /// <summary>
    ///     Gets the <see cref="string" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="string" />.</exception>
    public string String => GetValue<string>(OutputTage.String);

    /// <summary>
    ///     Gets the <see cref="bool" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="string" />.</exception>
    public bool Boolean => GetValue<bool>(OutputTage.Bool);

    /// <summary>
    ///     Gets the <see cref="double" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="double" />.</exception>
    public double Double => GetValue<double>(OutputTage.Double);

    /// <summary>
    ///     Gets the <see cref="long" /> value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="long" />.</exception>
    public long Long => GetValue<long>(OutputTage.Long);

    /// <summary>
    ///     Gets the <see cref="byte" /> array value.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="byte" /> array.</exception>
    public byte[] Bytes => GetValue<byte[]>(OutputTage.Bytes);

    /// <summary>
    ///     Gets the <see cref="Output" /> collection.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a <see cref="Output" /> collection.</exception>
    public JsonArray Collection => GetValue<JsonArray>(OutputTage.Collection);

    /// <summary>
    ///     Gets the value as json object.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a json object.</exception>
    public JsonObject Object => GetValue<JsonObject>(OutputTage.Object);

    /// <summary>
    ///     Gets the <see cref="Array" /> value.
    /// </summary>
    /// <returns>The resolved array.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Array" />.</exception>
    public Array Array => GetValue<Array>(OutputTage.Array);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved map.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    public Map Map => GetValue<Map>(OutputTage.Map);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    public Text Text => GetValue<Text>(OutputTage.Text);

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> value.
    /// </summary>
    /// <returns>The resolved xml element.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlElement" />.</exception>
    public XmlElement XmlElement => GetValue<XmlElement>(OutputTage.XmlElement);

    /// <summary>
    ///     Gets the <see cref="XmlText" /> value.
    /// </summary>
    /// <returns>The resolved xml text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlText" />.</exception>
    public XmlText XmlText => GetValue<XmlText>(OutputTage.XmlText);

    private static object? BuildValue(nint handle, uint length, Doc doc, OutputTage type)
    {
        switch (type)
        {
            case OutputTage.Bool:
                {
                    var value = OutputChannel.Boolean(handle).Checked();

                    return Marshal.PtrToStructure<byte>(value) == 1;
                }

            case OutputTage.Double:
                {
                    var value = OutputChannel.Double(handle).Checked();

                    return Marshal.PtrToStructure<double>(value);
                }

            case OutputTage.Long:
                {
                    var value = OutputChannel.Long(handle).Checked();

                    return Marshal.PtrToStructure<long>(value);
                }

            case OutputTage.String:
                {
                    MemoryReader.TryReadUtf8String(OutputChannel.String(handle), out var result);

                    return result;
                }

            case OutputTage.Bytes:
                {
                    var bytesHandle = OutputChannel.Bytes(handle).Checked();
                    var bytesArray = MemoryReader.ReadBytes(OutputChannel.Bytes(handle), length);

                    return bytesArray;
                }

            case OutputTage.Collection:
                {
                    return new JsonArray(handle, length, doc);
                }

            case OutputTage.Object:
                {
                    return new JsonObject(handle, length, doc);
                }

            case OutputTage.Array:
                return doc.GetArray(OutputChannel.Array(handle));

            case OutputTage.Map:
                return doc.GetMap(OutputChannel.Map(handle));

            case OutputTage.Text:
                return doc.GetText(OutputChannel.Text(handle));

            case OutputTage.XmlElement:
                return doc.GetXmlElement(OutputChannel.XmlElement(handle));

            case OutputTage.XmlText:
                return doc.GetXmlText(OutputChannel.XmlText(handle));

            case OutputTage.Doc:
                return doc.GetDoc(OutputChannel.Doc(handle));

            default:
                return null;
        }
    }

    private T GetValue<T>(OutputTage expectedType)
    {
        if (value is not T typed)
        {
            throw new YDotNetException($"Expected {expectedType}, got {Tag}.");
        }

        return typed;
    }
}
