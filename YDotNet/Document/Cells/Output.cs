using System.Runtime.InteropServices;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using YDotNet.Native.Types.Maps;
using Array = YDotNet.Document.Types.Arrays.Array;

#pragma warning disable SA1623 // Property summary documentation should match accessors

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to read information from the storage.
/// </summary>
public class Output
{
    private readonly Lazy<object?> value;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Output" /> class.
    /// </summary>
    /// <param name="handle">The pointer to the native resource that represents the storage.</param>
    /// <param name="shouldDispose">Indicates if the memory has been allocated and needs to be disposed.</param>
    internal Output(nint handle, bool shouldDispose)
    {
        var native = Marshal.PtrToStructure<OutputNative>(handle.Checked());

        Type = (OutputType)native.Tag;

        // We use lazy because some types like Doc and Map need to be disposed and therefore they should not be allocated, if not needed.
        value = BuildValue(handle, native.Length, Type);

        if (shouldDispose)
        {
            OutputChannel.Destroy(handle);
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
    public Output[] Collection => GetValue<Output[]>(OutputType.Collection);

    /// <summary>
    ///     Gets the value as json object.
    /// </summary>
    /// <exception cref="YDotNetException">Value is not a json object.</exception>
    public IDictionary<string, Output>? Object => GetValue<IDictionary<string, Output>>(OutputType.Object);

    /// <summary>
    ///     Gets the <see cref="Array" /> value.
    /// </summary>
    /// <returns>The resolved array.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Array" />.</exception>
    /// <remarks>You are responsible to dispose the array, if you use this property.</remarks>
    public Array ResolveArray() => GetValue<Array>(OutputType.Array);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved map.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    /// <remarks>You are responsible to dispose the map, if you use this property.</remarks>
    public Map ResolveMap() => GetValue<Map>(OutputType.Map);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <returns>The resolved text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="Map" />.</exception>
    /// <remarks>You are responsible to dispose the text, if you use this property.</remarks>
    public Text ResolveText() => GetValue<Text>(OutputType.Text);

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> value.
    /// </summary>
    /// <returns>The resolved xml element.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlElement" />.</exception>
    /// <remarks>You are responsible to dispose the xml element, if you use this property.</remarks>
    public XmlElement ResolveXmlElement() => GetValue<XmlElement>(OutputType.XmlElement);

    /// <summary>
    ///     Gets the <see cref="XmlText" /> value.
    /// </summary>
    /// <returns>The resolved xml text.</returns>
    /// <exception cref="YDotNetException">Value is not a <see cref="XmlText" />.</exception>
    /// <remarks>You are responsible to dispose the xml text, if you use this property.</remarks>
    public XmlText ResolveXmlText() => GetValue<XmlText>(OutputType.XmlText);

    private static Lazy<object?> BuildValue(nint handle, uint length, OutputType type)
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
                    var pointer = OutputChannel.Bytes(handle).Checked();

                    var result = MemoryReader.TryReadBytes(OutputChannel.Bytes(handle), length) ??
                        throw new YDotNetException("Internal type mismatch, native library returns null.");

                    if (result == null)
                    {
                        throw new YDotNetException("Internal type mismatch, native library returns null.");
                    }

                    OutputChannel.Destroy(pointer);
                    return new Lazy<object?>(result);
                }

            case OutputType.Collection:
                {
                    var pointer = OutputChannel.Collection(handle).Checked();

                    var handles = MemoryReader.TryReadIntPtrArray(pointer, length, Marshal.SizeOf<OutputNative>())
                        ?? throw new YDotNetException("Internal type mismatch, native library returns null.");

                    var result = handles.Select(x => new Output(x, false)).ToArray();

                    OutputChannel.Destroy(pointer);
                    return new Lazy<object?>(result);
                }

            case OutputType.Object:
                {
                    var pointer = OutputChannel.Object(handle).Checked();

                    var handlesArray = MemoryReader.TryReadIntPtrArray(pointer, length, Marshal.SizeOf<MapEntryNative>())
                        ?? throw new YDotNetException("Internal type mismatch, native library returns null.");

                    var result = new Dictionary<string, Output>();

                    foreach (var itemHandle in handlesArray)
                    {
                        var (mapEntry, outputHandle) = MemoryReader.ReadMapEntryAndOutputHandle(itemHandle);
                        var mapEntryKey = MemoryReader.ReadUtf8String(mapEntry.Field);

                        result[mapEntryKey] = new Output(outputHandle, false);
                    }

                    OutputChannel.Destroy(pointer);
                    return new Lazy<object?>(result);
                }

            case OutputType.Array:
                return new Lazy<object?>(() => new Array(OutputChannel.Array(handle).Checked()));

            case OutputType.Map:
                return new Lazy<object?>(() => new Map(OutputChannel.Map(handle).Checked()));

            case OutputType.Text:
                return new Lazy<object?>(() => new Text(OutputChannel.Text(handle).Checked()));

            case OutputType.XmlElement:
                return new Lazy<object?>(() => new XmlElement(OutputChannel.XmlElement(handle).Checked()));

            case OutputType.XmlText:
                return new Lazy<object?>(() => new XmlText(OutputChannel.XmlText(handle).Checked()));

            case OutputType.Doc:
                return new Lazy<object?>(() => new Doc(OutputChannel.Doc(handle).Checked()));

            default:
                return new Lazy<object?>((object?)null);
        }
    }

    private T GetValue<T>(OutputType expectedType)
    {
        var resolvedValue = value.Value;

        if (resolvedValue is not T typed)
        {
            throw new YDotNetException($"Expected {expectedType}, got {Type}.");
        }

        return typed;
    }
}
