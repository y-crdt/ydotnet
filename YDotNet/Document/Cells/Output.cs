using System.Reflection.Metadata;
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
            value = new Lazy<object?>((object?)null);
            return;
        }

        var native = Marshal.PtrToStructure<OutputNative>(handle);

        Type = (OutputInputType)native.Tag;

        value = BuildValue(handle, native.Length, Type);

        if (shouldDispose)
        {
            OutputChannel.Destroy(handle);
        }
    }

    /// <summary>
    ///     Gets the <see cref="Doc" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public Doc Doc => GetValue<Doc>(OutputInputType.Doc);

    /// <summary>
    ///     Gets the <see cref="string" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public string String => GetValue<string>(OutputInputType.String);

    /// <summary>
    ///     Gets the <see cref="bool" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public bool Boolean => GetValue<bool>(OutputInputType.Bool);

    /// <summary>
    ///     Gets the <see cref="double" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public double Double => GetValue<double>(OutputInputType.Double);

    /// <summary>
    ///     Gets the <see cref="long" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public long Long => GetValue<long>(OutputInputType.Long);

    /// <summary>
    ///     Gets the <see cref="byte" /> array or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public byte[] Bytes => GetValue<byte[]>(OutputInputType.Bytes);

    /// <summary>
    ///     Gets the <see cref="Input" /> collection or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Output[] Collection => GetValue<Output[]>(OutputInputType.Collection);

    /// <summary>
    ///     Gets the <see cref="Input" /> dictionary or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public IDictionary<string, Output>? Object => GetValue<IDictionary<string, Output>>(OutputInputType.Object);

    /// <summary>
    ///     Gets a value indicating whether this output cell contains a <c>null</c> value.
    /// </summary>
    public bool Null => Type == OutputInputType.Null;

    /// <summary>
    ///     Gets a value indicating whether this output cell contains an <c>undefined</c> value.
    /// </summary>
    public bool Undefined => Type == OutputInputType.Undefined;

    /// <summary>
    ///     Gets the <see cref="Array" /> or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Array Array => GetValue<Array>(OutputInputType.Array);

    /// <summary>
    ///     Gets the <see cref="Types.Maps.Map" /> or <c>null</c> if this output cells contains a different
    ///     type stored.
    /// </summary>
    public Map Map => GetValue<Map>(OutputInputType.Map);

    /// <summary>
    ///     Gets the <see cref="Types.Texts.Text" /> or <c>null</c> if this output cells contains a different
    ///     type
    ///     stored.
    /// </summary>
    public Text Text => GetValue<Text>(OutputInputType.Text);

    /// <summary>
    ///     Gets the <see cref="Types.XmlElements.XmlElement" /> or <c>null</c> if this output cells contains
    ///     a different type stored.
    /// </summary>
    public XmlElement XmlElement => GetValue<XmlElement>(OutputInputType.XmlElement);

    /// <summary>
    ///     Gets the <see cref="Types.XmlTexts.XmlText" /> or <c>null</c> if this output cells contains a
    ///     different type stored.
    /// </summary>
    public XmlText? XmlText => GetValue<XmlText>(OutputInputType.XmlText);

    /// <summary>
    ///     Gets the type of the output.
    /// </summary>
    public OutputInputType Type { get; private set; }

    private static Lazy<object?> BuildValue(nint handle, uint length, OutputInputType type)
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
            case OutputInputType.Bool:
                {
                    var value = GuardHandle(OutputChannel.Boolean(handle));

                    return new Lazy<object?>((object?)(Marshal.PtrToStructure<byte>(value) == 1));
                }

            case OutputInputType.Double:
                {
                    var value = GuardHandle(OutputChannel.Double(handle));

                    return new Lazy<object?>(Marshal.PtrToStructure<double>(value));
                }

            case OutputInputType.Long:
                {
                    var value = GuardHandle(OutputChannel.Long(handle));

                    return new Lazy<object?>(Marshal.PtrToStructure<long>(value));
                }

            case OutputInputType.String:
                {
                    MemoryReader.TryReadUtf8String(OutputChannel.String(handle), out var result);

                    return new Lazy<object?>(result);
                }

            case OutputInputType.Bytes:
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

            case OutputInputType.Collection:
                {
                    var pointer = GuardHandle(OutputChannel.Collection(handle));

                    var handles = MemoryReader.TryReadIntPtrArray(pointer, length, Marshal.SizeOf<OutputNative>())
                        ?? throw new InvalidOperationException("Internal type mismatch, native library returns null.");

                    var result = handles.Select(x => new Output(x, false)).ToArray();

                    OutputChannel.Destroy(pointer);
                    return new Lazy<object?>(result);
                }

            case OutputInputType.Object:
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

            case OutputInputType.Array:
                return new Lazy<object?>(() => new Array(GuardHandle(OutputChannel.Array(handle))));

            case OutputInputType.Map:
                return new Lazy<object?>(() => new Map(GuardHandle(OutputChannel.Map(handle))));

            case OutputInputType.Text:
                return new Lazy<object?>(() => new Text(GuardHandle(OutputChannel.Text(handle))));

            case OutputInputType.XmlElement:
                return new Lazy<object?>(() => new XmlElement(GuardHandle(OutputChannel.XmlElement(handle))));

            case OutputInputType.XmlText:
                return new Lazy<object?>(() => new XmlText(GuardHandle(OutputChannel.XmlText(handle))));

            case OutputInputType.Doc:
                return new Lazy<object?>(() => new Doc(GuardHandle(OutputChannel.Doc(handle))));

            default:
                return new Lazy<object?>((object?)null);
        }
    }

    private T GetValue<T>(OutputInputType expectedType)
    {
        var resolvedValue = value.Value;

        if (resolvedValue is not T typed)
        {
            throw new InvalidOperationException($"Expected {expectedType}, got {Type}.");
        }

        return typed;
    }
}
