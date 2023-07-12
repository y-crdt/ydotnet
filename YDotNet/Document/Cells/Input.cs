using YDotNet.Infrastructure;
using YDotNet.Native.Cells.Inputs;

namespace YDotNet.Document.Cells;

/// <summary>
///     Represents a cell used to send information to be stored.
/// </summary>
public sealed class Input
{
    private Input(InputNative inputNative)
    {
        InputNative = inputNative;
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
        // TODO [LSViana] Free the memory allocated here.
        return new Input(InputChannel.String(MemoryWriter.WriteUtf8String(value).Pointer));
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
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Bytes(byte[] value)
    {
        return new Input(InputChannel.Bytes(value, (uint) value.Length));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Collection(Input[] value)
    {
        var inputs = value.Select(x => x.InputNative).ToArray();

        // TODO [LSViana] Free the memory allocated here.
        return new Input(InputChannel.Collection(MemoryWriter.WriteStructArray(inputs), (uint) value.Length));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Input" /> class.
    /// </summary>
    /// <param name="value">The <see cref="long" /> value to be stored in the cell.</param>
    /// <returns>The <see cref="Input" /> cell that represents the provided value.</returns>
    public static Input Object(IDictionary<string, Input> value)
    {
        // TODO [LSViana] Free the memory allocated here.
        var keys = MemoryWriter.WriteUtf8StringArray(value.Keys.ToArray());
        var values = MemoryWriter.WriteStructArray(value.Values.Select(x => x.InputNative).ToArray());

        return new Input(InputChannel.Object(keys, values, (uint) value.Count));
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
        var inputs = value.Select(x => x.InputNative).ToArray();

        // TODO [LSViana] Free the memory allocated here.
        return new Input(InputChannel.Array(MemoryWriter.WriteStructArray(inputs), (uint) value.Length));
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

        return new Input(InputChannel.Map(keys, values, (uint) value.Count));
    }
}
