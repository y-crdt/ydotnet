using System.Runtime.InteropServices;
using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Outputs;

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
    ///     Gets a value indicating whether this output cell contains a <c>null</c> value.
    /// </summary>
    public bool Null => OutputChannel.Null(Handle);

    /// <summary>
    ///     Gets a value indicating whether this output cell contains an <c>undefined</c> value.
    /// </summary>
    public bool Undefined => OutputChannel.Undefined(Handle);

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
