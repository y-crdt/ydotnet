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
        if (handle == nint.Zero)
        {
            throw new ArgumentException("Handle cannot be zero.", nameof(handle));
        }

        var native = Marshal.PtrToStructure<OutputNative>(handle);

        Type = (OutputType)native.Tag;

        // We use lazy because some types like Doc and Map need to be disposed and therefore they should not be allocated, if not needed.
        value = BuildValue(handle, native.Length, Type);

        if (shouldDispose)
        {
            OutputChannel.Destroy(handle);
        }
    }

    /// <summary>
    ///     Gets the <see cref="Doc" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Doc" />.</exception>
    public Doc Doc => GetValue<Doc>(OutputType.Doc);

    /// <summary>
    ///     Gets the <see cref="string" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="string" />.</exception>
    public string String => GetValue<string>(OutputType.String);

    /// <summary>
    ///     Gets the <see cref="bool" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="string" />.</exception>
    public bool Boolean => GetValue<bool>(OutputType.Bool);

    /// <summary>
    ///     Gets the <see cref="double" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="double" />.</exception>
    public double Double => GetValue<double>(OutputType.Double);

    /// <summary>
    ///     Gets the <see cref="long" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="long" />.</exception>
    public long Long => GetValue<long>(OutputType.Long);

    /// <summary>
    ///     Gets the <see cref="byte" /> array value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="byte" /> array.</exception>
    public byte[] Bytes => GetValue<byte[]>(OutputType.Bytes);

    /// <summary>
    ///     Gets the <see cref="Output" /> collection.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Output" /> collection.</exception>
    public Output[] Collection => GetValue<Output[]>(OutputType.Collection);

    /// <summary>
    ///     Gets the value as json object.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a json object.</exception>
    public IDictionary<string, Output>? Object => GetValue<IDictionary<string, Output>>(OutputType.Object);

    /// <summary>
    ///     Gets the <see cref="Array" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Array" />.</exception>
    public Array Array => GetValue<Array>(OutputType.Array);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Map" />.</exception>
    public Map Map => GetValue<Map>(OutputType.Map);

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Map" />.</exception>
    public Text Text => GetValue<Text>(OutputType.Text);

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="XmlElement" />.</exception>
    public XmlElement XmlElement => GetValue<XmlElement>(OutputType.XmlElement);

    /// <summary>
    ///     Gets the <see cref="XmlText" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="XmlText" />.</exception>
    public XmlText XmlText => GetValue<XmlText>(OutputType.XmlText);

    /// <summary>
    ///     Gets the type of the output.
    /// </summary>
    public OutputType Type { get; private set; }

    private static Lazy<object?> BuildValue(nint handle, uint length, OutputType type)
    {
        static nint GuardHandle(nint handle)
        {
            if (handle == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return handle;
        }

        switch (type)
        {
            case OutputType.Bool:
                {
                    var value = GuardHandle(OutputChannel.Boolean(handle));

                    return new Lazy<object?>((object?)(Marshal.PtrToStructure<byte>(value) == 1));
                }

            case OutputType.Double:
                {
                    var value = GuardHandle(OutputChannel.Double(handle));

                    return new Lazy<object?>(Marshal.PtrToStructure<double>(value));
                }

            case OutputType.Long:
                {
                    var value = GuardHandle(OutputChannel.Long(handle));

                    return new Lazy<object?>(Marshal.PtrToStructure<long>(value));
                }

            case OutputType.String:
                {
                    MemoryReader.TryReadUtf8String(OutputChannel.String(handle), out var result);

                    return new Lazy<object?>(result);
                }

            case OutputType.Bytes:
                {
                    var pointer = GuardHandle(OutputChannel.Bytes(handle));

                    var result = MemoryReader.TryReadBytes(OutputChannel.Bytes(handle), length) ??
                        throw new InvalidOperationException("Internal type mismatch, native library returns null.");

                    if (result == null)
                    {
                        throw new InvalidOperationException("Internal type mismatch, native library returns null.");
                    }

                    OutputChannel.Destroy(pointer);
                    return new Lazy<object?>(result);
                }

            case OutputType.Collection:
                {
                    var pointer = GuardHandle(OutputChannel.Collection(handle));

                    var handles = MemoryReader.TryReadIntPtrArray(pointer, length, Marshal.SizeOf<OutputNative>())
                        ?? throw new InvalidOperationException("Internal type mismatch, native library returns null.");

                    var result = handles.Select(x => new Output(x, false)).ToArray();

                    OutputChannel.Destroy(pointer);
                    return new Lazy<object?>(result);
                }

            case OutputType.Object:
                {
                    var pointer = GuardHandle(OutputChannel.Object(handle));

                    var handlesArray = MemoryReader.TryReadIntPtrArray(pointer, length, Marshal.SizeOf<MapEntryNative>())
                        ?? throw new InvalidOperationException("Internal type mismatch, native library returns null.");

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
                return new Lazy<object?>(() => new Array(GuardHandle(OutputChannel.Array(handle))));

            case OutputType.Map:
                return new Lazy<object?>(() => new Map(GuardHandle(OutputChannel.Map(handle))));

            case OutputType.Text:
                return new Lazy<object?>(() => new Text(GuardHandle(OutputChannel.Text(handle))));

            case OutputType.XmlElement:
                return new Lazy<object?>(() => new XmlElement(GuardHandle(OutputChannel.XmlElement(handle))));

            case OutputType.XmlText:
                return new Lazy<object?>(() => new XmlText(GuardHandle(OutputChannel.XmlText(handle))));

            case OutputType.Doc:
                return new Lazy<object?>(() => new Doc(GuardHandle(OutputChannel.Doc(handle))));

            default:
                return new Lazy<object?>((object?)null);
        }
    }

    private T GetValue<T>(OutputType expectedType)
    {
        var resolvedValue = value.Value;

        if (resolvedValue is not T typed)
        {
            throw new InvalidOperationException($"Expected {expectedType}, got {Type}.");
        }

        return typed;
    }
}
