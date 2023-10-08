using System.Formats.Asn1;
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

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to read information from the storage.
/// </summary>
public class Output : IDisposable
{
    private readonly bool disposable;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Output" /> class.
    /// </summary>
    /// <param name="handle">The pointer to the native resource that represents the storage.</param>
    /// <param name="disposable">
    ///     The flag determines if the resource associated with <see cref="Handle" /> should be disposed
    ///     by this <see cref="Output" /> instance.
    /// </param>
    internal Output(nint handle, bool disposable = false)
    {
        this.disposable = disposable;

        Handle = handle;
        OutputNative = handle == nint.Zero ? null : Marshal.PtrToStructure<OutputNative>(handle);
    }

    /// <summary>
    ///     Gets the <see cref="Doc" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Doc" />.</exception>
    public Doc Doc
    {
        get
        {
            EnsureType(OutputType.Doc);

            return ReferenceAccessor.Access(new Doc(OutputChannel.Doc(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="string" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="string" />.</exception>
    public string String
    {
        get
        {
            EnsureType(OutputType.String);

            MemoryReader.TryReadUtf8String(OutputChannel.String(Handle), out var result);

            return result ?? throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="bool" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="bool" />.</exception>
    public bool Boolean
    {
        get
        {
            EnsureType(OutputType.Bool);

            var value = OutputChannel.Boolean(Handle);

            if (value == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return Marshal.PtrToStructure<byte>(value) == 1;
        }
    }

    /// <summary>
    ///     Gets the <see cref="double" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="double" />.</exception>
    public double Double
    {
        get
        {
            EnsureType(OutputType.Double);

            var value = OutputChannel.Double(Handle);

            if (value == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return Marshal.PtrToStructure<double>(value);
        }
    }

    /// <summary>
    ///     Gets the <see cref="long" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="long" />.</exception>
    public long Long
    {
        get
        {
            EnsureType(OutputType.Long);

            var value = OutputChannel.Long(Handle);

            if (value == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return Marshal.PtrToStructure<long>(value);
        }
    }

    /// <summary>
    ///     Gets the <see cref="byte[]" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="byte[]" />.</exception>
    public byte[] Bytes
    {
        get
        {
            EnsureType(OutputType.Bytes);

            var result = MemoryReader.TryReadBytes(OutputChannel.Bytes(Handle), OutputNative.Value.Length) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");

            if (result == null)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return result;
        }
    }

    /// <summary>
    ///     Gets the <see cref="Output[]" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Output[]" />.</exception>
    public Output[] Collection
    {
        get
        {
            EnsureType(OutputType.Collection);

            var handles = MemoryReader.TryReadIntPtrArray(
                OutputChannel.Collection(Handle), OutputNative!.Value.Length, Marshal.SizeOf<OutputNative>());

            if (handles == null)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return handles.Select(x => new Output(x)).ToArray();
        }
    }

    /// <summary>
    ///     Gets the JsonObject value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a JsonObject.</exception>
    public IDictionary<string, Output>? Object
    {
        get
        {
            EnsureType(OutputType.Object);

            var handles = MemoryReader.TryReadIntPtrArray(
                OutputChannel.Object(Handle), OutputNative!.Value.Length, Marshal.SizeOf<MapEntryNative>());

            if (handles == null)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            var result = new Dictionary<string, Output>();

            foreach (var handle in handles)
            {
                var (mapEntry, outputHandle) = MemoryReader.ReadMapEntryAndOutputHandle(handle);
                var mapEntryKey = MemoryReader.ReadUtf8String(mapEntry.Field);

                result[mapEntryKey] = new Output(outputHandle);
            }

            return result;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this output cell contains a <c>null</c> value.
    /// </summary>
    public bool Null => Type == OutputType.Null;

    /// <summary>
    ///     Gets a value indicating whether this output cell contains an <c>undefined</c> value.
    /// </summary>
    public bool Undefined => Type == OutputType.Undefined;

    /// <summary>
    ///     Gets the <see cref="Array" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Array" />.</exception>
    public Array Array
    {
        get
        {
            EnsureType(OutputType.Array);

            return ReferenceAccessor.Access(new Array(OutputChannel.Array(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="Map" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Map" />.</exception>
    public Map Map
    {
        get
        {
            EnsureType(OutputType.Map);

            return ReferenceAccessor.Access(new Map(OutputChannel.Map(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="Text" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="Text" />.</exception>
    public Text Text
    {
        get
        {
            EnsureType(OutputType.Text);

            return ReferenceAccessor.Access(new Text(OutputChannel.Text(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="XmlElement" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="XmlElement" />.</exception>
    public XmlElement XmlElement
    {
        get
        {
            EnsureType(OutputType.XmlElement);

            return ReferenceAccessor.Access(new XmlElement(OutputChannel.XmlElement(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="XmlText" /> value.
    /// </summary>
    /// <exception cref="InvalidOperationException">Value is not a <see cref="XmlText" />.</exception>
    public XmlText XmlText
    {
        get
        {
            EnsureType(OutputType.XmlText);

            return ReferenceAccessor.Access(new XmlText(OutputChannel.XmlText(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the type of the output.
    /// </summary>
    public OutputType Type => (OutputType)(OutputNative?.Tag ?? -99);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the native output cell represented by this cell.
    /// </summary>
    private OutputNative? OutputNative { get; }

    private void EnsureType(OutputType expectedType)
    {
        if (Type != expectedType)
        {
            throw new InvalidOperationException($"Expected {expectedType}, got {Type}.");
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!disposable)
        {
            return;
        }

        OutputChannel.Destroy(Handle);
    }
}
