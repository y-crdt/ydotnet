using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to send information to be stored.
/// </summary>
public abstract class Input : IDisposable
{
    /// <summary>
    ///     Gets or sets the native input cell represented by this cell.
    /// </summary>
    public InputNative InputNative { get; set; }

    /// <inheritdoc />
    public abstract void Dispose();

    /// <summary>
    ///     Creates a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="Doc" /> instance to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Doc(Doc value)
    {
        return new InputEmpty(InputChannel.Doc(value.Handle));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="string" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input String(string value)
    {
        var pointer = MemoryWriter.WriteUtf8String(value);

        return new InputPointer(InputChannel.String(pointer), pointer);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="bool" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Boolean(bool value)
    {
        return new InputEmpty(InputChannel.Boolean(value));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="double" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Double(double value)
    {
        return new InputEmpty(InputChannel.Double(value));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Long(long value)
    {
        return new InputEmpty(InputChannel.Long(value));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="byte" /> array value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Bytes(byte[] value)
    {
        return new InputEmpty(InputChannel.Bytes(value, (uint) value.Length));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="Input" /> array value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Collection(Input[] value)
    {
        var inputs = value.Select(x => x.InputNative).ToArray();
        var pointer = MemoryWriter.WriteStructArray(inputs);

        return new InputPointer(InputChannel.Collection(pointer, (uint) value.Length), pointer);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="Dictionary{TKey,TValue}" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Object(IDictionary<string, Input> value)
    {
        var keys = MemoryWriter.WriteUtf8StringArray(value.Keys.ToArray());
        var values = MemoryWriter.WriteStructArray(value.Values.Select(x => x.InputNative).ToArray());

        return new InputPointerArray(
            InputChannel.Object(keys.Head, values, (uint) value.Count),
            keys.Pointers.Concat(new[] { keys.Head, values }).ToArray());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Null()
    {
        return new InputEmpty(InputChannel.Null());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Undefined()
    {
        return new InputEmpty(InputChannel.Undefined());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Array(Input[] value)
    {
        var inputs = value.Select(x => x.InputNative).ToArray();
        var pointer = MemoryWriter.WriteStructArray(inputs);

        return new InputPointer(InputChannel.Array(pointer, (uint) value.Length), pointer);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Map(IDictionary<string, Input> value)
    {
        var keys = MemoryWriter.WriteUtf8StringArray(value.Keys.ToArray());
        var values = MemoryWriter.WriteStructArray(value.Values.Select(x => x.InputNative).ToArray());

        return new InputPointerArray(
            InputChannel.Map(keys.Head, values, (uint) value.Count),
            keys.Pointers.Concat(new[] { keys.Head, values }).ToArray());
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Text(string value)
    {
        var pointer = MemoryWriter.WriteUtf8String(value);

        return new InputPointer(InputChannel.Text(pointer), pointer);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="name">The <see cref="string" /> value to name the <see cref="Types.XmlElement" /> in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input XmlElement(string name)
    {
        var pointer = MemoryWriter.WriteUtf8String(name);

        return new InputPointer(InputChannel.XmlElement(pointer), pointer);
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="value" /> value to fill the <see cref="Types.XmlText" /> in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input XmlText(string value)
    {
        var pointer = MemoryWriter.WriteUtf8String(value);

        return new InputPointer(InputChannel.XmlText(pointer), pointer);
    }
}
