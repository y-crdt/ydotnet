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
public sealed class Output : UnmanagedResource
{
    private readonly Lazy<object?> value;

    internal Output(nint handle, Doc doc, IResourceOwner? owner)
        : base(handle, owner)
    {
        var native = Marshal.PtrToStructure<OutputNative>(handle.Checked());

        Type = (OutputType)native.Tag;

        // We use lazy because some types like Doc and Map need to be disposed and therefore they should not be allocated, if not needed.
        value = BuildValue(handle, native.Length, doc, Type, this);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Output"/> class.
    /// </summary>
    ~Output()
    {
        Dispose(true);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        if (Owner == null)
        {
            OutputChannel.Destroy(Handle);
        }
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

    private static Lazy<object?> BuildValue(nint handle, uint length, Doc doc, OutputType type, IResourceOwner owner)
    {
        switch (type)
        {
            case OutputType.Bool:
                {
                    var value = OutputChannel.Boolean(handle).Checked();

                    return new Lazy<object?>((object?)(Marshal.PtrToStructure<byte>(value) == 1));
                }

            case OutputType.Double:
                {
                    var value = OutputChannel.Double(handle).Checked();

                    return new Lazy<object?>(Marshal.PtrToStructure<double>(value));
                }

            case OutputType.Long:
                {
                    var value = OutputChannel.Long(handle).Checked();

                    return new Lazy<object?>(Marshal.PtrToStructure<long>(value));
                }

            case OutputType.String:
                {
                    MemoryReader.TryReadUtf8String(OutputChannel.String(handle), out var result);

                    return new Lazy<object?>(result);
                }

            case OutputType.Bytes:
                {
                    var bytesHandle = OutputChannel.Bytes(handle).Checked();
                    var bytesArray = MemoryReader.ReadBytes(OutputChannel.Bytes(handle), length);

                    return new Lazy<object?>(bytesArray);
                }

            case OutputType.Collection:
                {
                    return new Lazy<object?>(() => new JsonArray(handle, length, doc, owner));
                }

            case OutputType.Object:
                {
                    return new Lazy<object?>(() => new JsonObject(handle, length, doc, owner));
                }

            case OutputType.Array:
                return new Lazy<object?>(() => doc.GetArray(OutputChannel.Array(handle)));

            case OutputType.Map:
                return new Lazy<object?>(() => doc.GetMap(OutputChannel.Map(handle)));

            case OutputType.Text:
                return new Lazy<object?>(() => doc.GetText(OutputChannel.Text(handle)));

            case OutputType.XmlElement:
                return new Lazy<object?>(() => doc.GetXmlElement(OutputChannel.XmlElement(handle)));

            case OutputType.XmlText:
                return new Lazy<object?>(() => doc.GetXmlText(OutputChannel.XmlText(handle)));

            case OutputType.Doc:
                return new Lazy<object?>(() => doc.GetDoc(OutputChannel.Doc(handle)));

            default:
                return new Lazy<object?>((object?)null);
        }
    }

    private T GetValue<T>(OutputType expectedType)
    {
        ThrowIfDisposed();

        var resolvedValue = value.Value;

        if (resolvedValue is not T typed)
        {
            throw new YDotNetException($"Expected {expectedType}, got {Type}.");
        }

        return typed;
    }
}
