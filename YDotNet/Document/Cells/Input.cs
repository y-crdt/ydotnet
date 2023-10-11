using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to send information to be stored.
/// </summary>
public sealed class Input : Resource
{
    private readonly IDisposable[] allocatedMemory;

    internal Input(InputNative native, params IDisposable[] allocatedMemory)
    {
        this.allocatedMemory = allocatedMemory;

        InputNative = native;
    }

    protected override void DisposeCore(bool disposing)
    {
        foreach (var memory in allocatedMemory)
        {
            memory.Dispose();
        }
    }

    /// <summary>
    ///     Gets the native input cell represented by this cell.
    /// </summary>
    internal InputNative InputNative { get; }

    /// <summary>
    ///     Creates a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="Doc" /> instance to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Doc(Doc value)
    {
        return new Input(InputChannel.Doc(value.Handle));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="string" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input String(string value)
    {
        var unsafeValue = MemoryWriter.WriteUtf8String(value);

        return new Input(InputChannel.String(unsafeValue.Handle), unsafeValue);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="bool" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Boolean(bool value)
    {
        return new Input(InputChannel.Boolean(value));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="double" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Double(double value)
    {
        return new Input(InputChannel.Double(value));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Long(long value)
    {
        return new Input(InputChannel.Long(value));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="byte" /> array value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Bytes(byte[] value)
    {
        return new Input(InputChannel.Bytes(value, (uint)value.Length));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="Input" /> array value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Collection(Input[] value)
    {
        var unsafeMemory = MemoryWriter.WriteStructArray(value.Select(x => x.InputNative).ToArray());

        return new Input(InputChannel.Collection(unsafeMemory.Handle, (uint)value.Length), unsafeMemory);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="Dictionary{TKey,TValue}" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Object(IDictionary<string, Input> value)
    {
        var unsageKeys = MemoryWriter.WriteUtf8StringArray(value.Keys.ToArray());
        var unsafeValues = MemoryWriter.WriteStructArray(value.Values.Select(x => x.InputNative).ToArray());

        return new Input(InputChannel.Object(unsageKeys.Handle, unsafeValues.Handle, (uint)value.Count), unsageKeys, unsafeValues);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Null()
    {
        return new Input(InputChannel.Null());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Undefined()
    {
        return new Input(InputChannel.Undefined());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Array(Input[] value)
    {
        var unsafeMemory = MemoryWriter.WriteStructArray(value.Select(x => x.InputNative).ToArray());

        return new Input(InputChannel.Array(unsafeMemory.Handle, (uint)value.Length), unsafeMemory);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Map(IDictionary<string, Input> value)
    {
        var unsageKeys = MemoryWriter.WriteUtf8StringArray(value.Keys.ToArray());
        var unsafeValues = MemoryWriter.WriteStructArray(value.Values.Select(x => x.InputNative).ToArray());

        return new Input(InputChannel.Map(unsageKeys.Handle, unsafeValues.Handle, (uint)value.Count), unsageKeys, unsafeValues);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Text(string value)
    {
        var unsafeValue = MemoryWriter.WriteUtf8String(value);

        return new Input(InputChannel.Text(unsafeValue.Handle), unsafeValue);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="name">The <see cref="string" /> value to name the <see cref="Types.XmlElement" /> in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input XmlElement(string name)
    {
        var unsafeValue = MemoryWriter.WriteUtf8String(name);

        return new Input(InputChannel.XmlElement(unsafeValue.Handle), unsafeValue);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="value" /> value to fill the <see cref="Types.XmlText" /> in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input XmlText(string value)
    {
        var unsafeValue = MemoryWriter.WriteUtf8String(value);

        return new Input(InputChannel.XmlText(unsafeValue.Handle), unsafeValue);
    }
}
