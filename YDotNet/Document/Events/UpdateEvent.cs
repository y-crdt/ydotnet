using System.Runtime.InteropServices;

namespace YDotNet.Document.Events;

/// <summary>
///     An update event passed to a callback subscribed to <see cref="Doc.ObserveUpdatesV1" /> or
///     <see cref="Doc.ObserveUpdatesV2" />.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public class UpdateEvent
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="UpdateEvent" /> class.
    /// </summary>
    /// <param name="update">The binary information this event represents.</param>
    public UpdateEvent(byte[] update)
    {
        Update = update;
    }

    /// <summary>
    ///     Gets the binary information about all inserted and deleted changes performed within the scope of its transaction.
    /// </summary>
    public byte[] Update { get; }

    /// <summary>
    ///     Initializes an <see cref="UpdateEvent" /> instance from length and pointer to data.
    /// </summary>
    /// <param name="length">The length of the data to be read.</param>
    /// <param name="data">The pointer to the data.</param>
    /// <returns>An instance of <see cref="UpdateEvent" /> with the data loaded in it.</returns>
    public static UpdateEvent From(uint length, nint data)
    {
        var result = new byte[length];
        Marshal.Copy(data, result, startIndex: 0, (int) length);

        return new UpdateEvent(result);
    }
}
