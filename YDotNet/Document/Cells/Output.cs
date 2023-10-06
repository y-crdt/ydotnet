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
    ///     Gets the <see cref="Doc" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public Doc? Doc => ReferenceAccessor.Access(new Doc(OutputChannel.Doc(Handle)));

    /// <summary>
    ///     Gets the <see cref="string" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public string? String
    {
        get
        {
            EnsureType(OutputInputType.String);

            MemoryReader.TryReadUtf8String(OutputChannel.String(Handle), out var result);

            return result;
        }
    }

    /// <summary>
    ///     Gets the <see cref="bool" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public bool? Boolean
    {
        get
        {
            EnsureType(OutputInputType.Bool);

            var value = OutputChannel.Boolean(Handle);

            if (value == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return Marshal.PtrToStructure<byte>(value) == 1;
        }
    }

    /// <summary>
    ///     Gets the <see cref="double" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public double? Double
    {
        get
        {
            EnsureType(OutputInputType.Double);

            var value = OutputChannel.Double(Handle);

            if (value == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return Marshal.PtrToStructure<double>(value);
        }
    }

    /// <summary>
    ///     Gets the <see cref="long" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public long? Long
    {
        get
        {
            EnsureType(OutputInputType.Long);

            var value = OutputChannel.Long(Handle);

            if (value == nint.Zero)
            {
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
            }

            return Marshal.PtrToStructure<long>(value);
        }
    }

    /// <summary>
    ///     Gets the <see cref="byte" /> array or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public byte[] Bytes
    {
        get
        {
            EnsureType(OutputInputType.Bytes);

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
    ///     Gets the <see cref="Input" /> collection or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Output[] Collection
    {
        get
        {
            EnsureType(OutputInputType.Collection);

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
    ///     Gets the <see cref="Input" /> dictionary or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public IDictionary<string, Output>? Object
    {
        get
        {
            EnsureType(OutputInputType.Object);

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
    public bool Null => Type == OutputInputType.Null;

    /// <summary>
    ///     Gets a value indicating whether this output cell contains an <c>undefined</c> value.
    /// </summary>
    public bool Undefined => Type == OutputInputType.Undefined;

    /// <summary>
    ///     Gets the <see cref="Array" /> or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Array Array
    {
        get
        {
            EnsureType(OutputInputType.Array);

            return ReferenceAccessor.Access(new Array(OutputChannel.Array(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="YDotNet.Document.Types.Maps.Map" /> or <c>null</c> if this output cells contains a different
    ///     type stored.
    /// </summary>
    public Map Map
    {
        get
        {
            EnsureType(OutputInputType.Map);

            return ReferenceAccessor.Access(new Map(OutputChannel.Map(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="YDotNet.Document.Types.Texts.Text" /> or <c>null</c> if this output cells contains a different
    ///     type
    ///     stored.
    /// </summary>
    public Text Text
    {
        get
        {
            EnsureType(OutputInputType.Text);

            return ReferenceAccessor.Access(new Text(OutputChannel.Text(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="YDotNet.Document.Types.XmlElements.XmlElement" /> or <c>null</c> if this output cells contains
    ///     a different type stored.
    /// </summary>
    public XmlElement XmlElement
    {
        get
        {
            EnsureType(OutputInputType.XmlElement);

            return ReferenceAccessor.Access(new XmlElement(OutputChannel.XmlElement(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the <see cref="YDotNet.Document.Types.XmlTexts.XmlText" /> or <c>null</c> if this output cells contains a
    ///     different type stored.
    /// </summary>
    public XmlText? XmlText
    {
        get
        {
            EnsureType(OutputInputType.XmlText);

            return ReferenceAccessor.Access(new XmlText(OutputChannel.XmlText(Handle))) ??
                throw new InvalidOperationException("Internal type mismatch, native library returns null.");
        }
    }

    /// <summary>
    ///     Gets the type of the output.
    /// </summary>
    public OutputInputType Type => (OutputInputType)(OutputNative?.Tag ?? -99);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the native output cell represented by this cell.
    /// </summary>
    private OutputNative? OutputNative { get; }

    private void EnsureType(OutputInputType expectedType)
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
