using System.Runtime.InteropServices;
using YDotNet.Native.Transaction;

namespace YDotNet.Document.Transactions;

/// <summary>
///     Represents operations that are executed against a document. This can be reading or writing data.
/// </summary>
/// <remarks>
///     All operations that need to touch or modify the contents of a document need to be executed through a transaction.
/// </remarks>
public class Transaction
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Transaction" /> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    internal Transaction(nint handle)
    {
        Handle = handle;
    }

    /// <summary>
    ///     Gets a value indicating whether the transaction is writeable.
    /// </summary>
    public bool Writeable => TransactionChannel.Writeable(Handle);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; }

    /// <summary>
    ///     Commit and dispose provided read-write transaction.
    /// </summary>
    /// <remarks>
    ///     This operation releases allocated resources, triggers update events and performs a storage compression over all
    ///     operations executed in scope of a current transaction.
    /// </remarks>
    public void Commit()
    {
        TransactionChannel.Commit(Handle);
    }

    /// <summary>
    ///     Returns the sub-documents existing within current document.
    /// </summary>
    /// <returns>The sub-documents existing within current document.</returns>
    public Doc[] SubDocs()
    {
        var handle = TransactionChannel.SubDocs(Handle, out var length);
        var docs = new Doc[length];

        for (var i = 0; i < length; i++)
        {
            var doc = new Doc(Marshal.ReadIntPtr(handle, i * nint.Size));
            docs[i] = doc;
        }

        return docs;
    }

    /// <summary>
    ///     Returns the state vector of the <see cref="Doc" /> associated to this <see cref="Transaction" />,
    ///     serialized using lib0 v1 encoding.
    /// </summary>
    /// <remarks>
    ///     Payload created by this function can then be send over the network to a remote peer,
    ///     where it can be used as a parameter to <see cref="StateDiffV1" /> in order to produce a delta
    ///     update payload, that can be sent back and applied locally in order to efficiently propagate
    ///     updates from one peer to another.
    /// </remarks>
    /// <returns>
    ///     The lib0 v1 encoded state vector of the <see cref="Doc" /> associated to this <see cref="Transaction" />.
    /// </returns>
    public byte[] StateVectorV1()
    {
        var handle = TransactionChannel.StateVectorV1(Handle, out var length);
        var data = ReadBytes(handle, length);

        return data;
    }

    /// <summary>
    ///     Returns the state difference between current state of the <see cref="Doc" /> associated to this
    ///     <see cref="Transaction" /> and a state vector.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="stateVector" /> sent to this method can be generated using <see cref="StateVectorV1" />
    ///         through a <see cref="Transaction" /> in the remote document.
    ///     </para>
    ///     <para>
    ///         Such state difference can be sent back to the <see cref="Transaction" /> of the sender in order to propagate
    ///         and apply (using <see cref="ApplyV1" />) all updates known to a current document.
    ///     </para>
    ///     <para>
    ///         This method uses lib0 v1 encoding.
    ///     </para>
    /// </remarks>
    /// <param name="stateVector">
    ///     The state vector to be used as base for comparison and generation of the difference.
    /// </param>
    /// <returns>
    ///     The lib0 v1 encoded state difference between the <see cref="Doc" /> of this <see cref="Transaction" /> and the
    ///     remote <see cref="Doc" />.
    /// </returns>
    public byte[] StateDiffV1(byte[] stateVector)
    {
        var handle = TransactionChannel.StateDiffV1(Handle, stateVector, (uint) stateVector.Length, out var length);
        var data = ReadBytes(handle, length);

        return data;
    }

    /// <summary>
    ///     Returns the state difference between current state of the <see cref="Doc" /> associated to this
    ///     <see cref="Transaction" /> and a state vector.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <see cref="stateVector" /> sent to this method can be generated using <see cref="StateVectorV1" />
    ///         through a <see cref="Transaction" /> in the remote document.
    ///     </para>
    ///     <para>
    ///         Such state difference can be sent back to the <see cref="Transaction" /> of the sender in order to propagate
    ///         and apply (using <see cref="ApplyV2" />) all updates known to a current document.
    ///     </para>
    ///     <para>
    ///         This method uses lib0 v2 encoding.
    ///     </para>
    /// </remarks>
    /// <param name="stateVector">
    ///     The state vector to be used as base for comparison and generation of the difference.
    /// </param>
    /// <returns>
    ///     The lib0 v2 encoded state difference between the <see cref="Doc" /> of this <see cref="Transaction" /> and the
    ///     remote <see cref="Doc" />.
    /// </returns>
    public byte[] StateDiffV2(byte[] stateVector)
    {
        var handle = TransactionChannel.StateDiffV2(Handle, stateVector, (uint) stateVector.Length, out var length);
        var data = ReadBytes(handle, length);

        return data;
    }

    /// <summary>
    ///     Applies a state difference update (generated by <see cref="StateDiffV1" />) to the <see cref="Doc" /> associated to
    ///     this <see cref="Transaction" />.
    /// </summary>
    /// <param name="stateDiff">
    ///     The state difference that was generated using <see cref="stateDiffV1" /> in the remote
    ///     document.
    /// </param>
    /// <returns>The status of the update operation.</returns>
    public TransactionUpdateResult ApplyV1(byte[] stateDiff)
    {
        return (TransactionUpdateResult) TransactionChannel.ApplyV1(Handle, stateDiff, (uint) stateDiff.Length);
    }

    // TODO [LSViana] Consider extracting this to an infrastructure class.
    private static unsafe byte[] ReadBytes(nint handle, uint length)
    {
        var data = new byte[length];
        var stream = new UnmanagedMemoryStream((byte*) handle.ToPointer(), length);
        int bytesRead;

        do
        {
            bytesRead = stream.Read(data, offset: 0, data.Length);
        }
        while (bytesRead < data.Length);

        stream.Dispose();

        return data;
    }
}
