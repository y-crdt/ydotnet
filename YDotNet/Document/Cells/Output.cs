using System.Runtime.InteropServices;
using YDotNet.Document.Types;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;
using YDotNet.Native.Types.Maps;
using Array = YDotNet.Document.Types.Array;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to read information from the storage.
/// </summary>
public class Output : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Output" /> class.
    /// </summary>
    /// <param name="handle">The pointer to the native resource that represents the storage.</param>
    internal Output(nint handle)
    {
        Handle = handle;

        // TODO [LSViana] Use `youtput_destroy` to release the native resource.
        OutputNative = Marshal.PtrToStructure<OutputNative>(handle);
    }

    /// <summary>
    ///     Gets the <see cref="Doc" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public Doc? Doc => ReferenceAccessor.Access(new Doc(OutputChannel.Doc(Handle)));

    /// <summary>
    ///     Gets the <see cref="string" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public string? String => OutputChannel.String(Handle);

    /// <summary>
    ///     Gets the <see cref="bool" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public bool? Boolean
    {
        get
        {
            var value = OutputChannel.Boolean(Handle);

            return value == nint.Zero ? null : Marshal.PtrToStructure<byte>(value) == 1;
        }
    }

    /// <summary>
    ///     Gets the <see cref="double" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public double? Double
    {
        get
        {
            var value = OutputChannel.Double(Handle);

            return value == nint.Zero ? null : Marshal.PtrToStructure<double>(value);
        }
    }

    /// <summary>
    ///     Gets the <see cref="long" /> or <c>null</c> if this output cell contains a different type stored.
    /// </summary>
    public long? Long
    {
        get
        {
            var value = OutputChannel.Long(Handle);

            return value == nint.Zero ? null : Marshal.PtrToStructure<long>(value);
        }
    }

    /// <summary>
    ///     Gets the <see cref="byte" /> array or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public byte[]? Bytes => MemoryReader.TryReadBytes(OutputChannel.Bytes(Handle), OutputNative.Length);

    /// <summary>
    ///     Gets the <see cref="Input" /> collection or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Output[]? Collection =>
        MemoryReader.TryReadIntPtrArray(
                OutputChannel.Collection(Handle), OutputNative.Length, Marshal.SizeOf<OutputNative>())
            ?.Select(x => new Output(x))
            .ToArray();

    /// <summary>
    ///     Gets the <see cref="Input" /> dictionary or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public IDictionary<string, Output>? Object
    {
        get
        {
            // TODO [LSViana] Refactor this method to extract shared logic with other flows.
            var handles = MemoryReader.TryReadIntPtrArray(
                OutputChannel.Object(Handle), OutputNative.Length, Marshal.SizeOf<MapEntryNative>());

            if (handles == null)
            {
                return null;
            }

            // This pointer size is used to offset the `MapEntryNative.Field` value (which is a string pointer).
            var pointerSize = Marshal.SizeOf<nint>();
            var result = new Dictionary<string, Output>();

            foreach (var handle in handles)
            {
                var mapEntry = Marshal.PtrToStructure<MapEntryNative>(handle);
                var outputNativeHandle = handle + pointerSize;

                result[mapEntry.Field] = new Output(outputNativeHandle);
            }

            return result;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this output cell contains a <c>null</c> value.
    /// </summary>
    public bool Null => OutputChannel.Null(Handle) == 1;

    /// <summary>
    ///     Gets a value indicating whether this output cell contains an <c>undefined</c> value.
    /// </summary>
    public bool Undefined => OutputChannel.Undefined(Handle) == 1;

    /// <summary>
    ///     Gets the <see cref="Types.Array" /> or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Array? Array => ReferenceAccessor.Access(new Array(OutputChannel.Array(Handle)));

    /// <summary>
    ///     Gets the <see cref="Types.Array" /> or <c>null</c> if this output cells contains a different type stored.
    /// </summary>
    public Map? Map => ReferenceAccessor.Access(new Map(OutputChannel.Map(Handle)));

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Gets the native output cell represented by this cell.
    /// </summary>
    private OutputNative OutputNative { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        // TODO [LSViana] Use `youtput_destroy` here.
    }
}
