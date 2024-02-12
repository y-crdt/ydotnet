using YDotNet.Document.Options;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Infrastructure.Extensions;
using YDotNet.Native.Transaction;
using YDotNet.Native.Types.Branches;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Document.Transactions;

/// <summary>
///     Represents operations that are executed against a document. This can be reading or writing data.
/// </summary>
/// <remarks>
///     <para>
///         All operations that need to touch or modify the contents of a document need to be executed through a
///         transaction.
///     </para>
///     <para>A <see cref="Transaction" /> is automatically committed during <see cref="IDisposable.Dispose" />.</para>
/// </remarks>
public class Transaction : UnmanagedResource
{
    private readonly Doc doc;

    internal Transaction(nint handle, Doc doc)
        : base(handle)
    {
        this.doc = doc;

        doc.NotifyTransactionStarted();
    }

    /// <summary>
    ///     Gets a value indicating whether the transaction is writeable.
    /// </summary>
    public bool Writeable => TransactionChannel.Writeable(Handle) == 1;

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        if (disposing)
        {
            TransactionChannel.Commit(Handle);

            doc.NotifyTransactionClosed();
        }
    }

    /// <summary>
    ///     Commit and dispose provided read-write transaction.
    /// </summary>
    /// <remarks>
    ///     This operation releases allocated resources, triggers update events and performs a storage compression over all
    ///     operations executed in scope of a current transaction.
    /// </remarks>
    public void Commit()
    {
        // The dispose method has a solution to prevent multiple dipose, so we can just use that.
        Dispose();
    }

    /// <summary>
    ///     Returns the sub-documents existing within current document.
    /// </summary>
    /// <returns>The sub-documents existing within current document.</returns>
    public Doc[] SubDocs()
    {
        var handle = TransactionChannel.SubDocs(Handle, out var length);
        var handles = MemoryReader.ReadStructs<nint>(handle, length);

        return handles.Select(h => doc.GetDoc(h, isDeleted: false)).ToArray();
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

        return MemoryReader.ReadAndDestroyBytes(handle, length);
    }

    /// <summary>
    ///     Returns the state difference between current state of the <see cref="Doc" /> associated to this
    ///     <see cref="Transaction" /> and a state vector.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="stateVector" /> sent to this method can be generated using <see cref="StateVectorV1" />
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
    ///     The optional state vector to be used as base for comparison and generation of the difference.
    /// </param>
    /// <returns>
    ///     The lib0 v1 encoded state difference between the <see cref="Doc" /> of this <see cref="Transaction" /> and the
    ///     remote <see cref="Doc" />.
    /// </returns>
    public byte[] StateDiffV1(byte[]? stateVector)
    {
        var handle = TransactionChannel.StateDiffV1(
            Handle, stateVector, (uint)(stateVector?.Length ?? 0), out var length);

        return MemoryReader.ReadAndDestroyBytes(handle, length);
    }

    /// <summary>
    ///     Returns the state difference between current state of the <see cref="Doc" /> associated to this
    ///     <see cref="Transaction" /> and a state vector.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The <paramref name="stateVector" /> sent to this method can be generated using <see cref="StateVectorV1" />
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
    ///     The optional state vector to be used as base for comparison and generation of the difference.
    /// </param>
    /// <returns>
    ///     The lib0 v2 encoded state difference between the <see cref="Doc" /> of this <see cref="Transaction" /> and the
    ///     remote <see cref="Doc" />.
    /// </returns>
    public byte[] StateDiffV2(byte[]? stateVector)
    {
        var handle = TransactionChannel.StateDiffV2(
            Handle,
            stateVector,
            (uint)(stateVector?.Length ?? 0),
            out var length);

        return MemoryReader.ReadAndDestroyBytes(handle, length);
    }

    /// <summary>
    ///     Applies a state difference update (generated by <see cref="StateDiffV1" />) to the <see cref="Doc" /> associated to
    ///     this <see cref="Transaction" />.
    /// </summary>
    /// <param name="stateDiff">
    ///     The state difference that was generated using <see cref="StateDiffV1" /> in the remote
    ///     document.
    /// </param>
    /// <returns>The result of the update operation.</returns>
    public TransactionUpdateResult ApplyV1(byte[] stateDiff)
    {
        return (TransactionUpdateResult)TransactionChannel.ApplyV1(Handle, stateDiff, (uint)stateDiff.Length);
    }

    /// <summary>
    ///     Applies a state difference update (generated by <see cref="StateDiffV2" />) to the <see cref="Doc" /> associated to
    ///     this <see cref="Transaction" />.
    /// </summary>
    /// <param name="stateDiff">
    ///     The state difference that was generated using <see cref="StateDiffV2" /> in the remote
    ///     document.
    /// </param>
    /// <returns>The result of the update operation.</returns>
    public TransactionUpdateResult ApplyV2(byte[] stateDiff)
    {
        return (TransactionUpdateResult)TransactionChannel.ApplyV2(Handle, stateDiff, (uint)stateDiff.Length);
    }

    /// <summary>
    ///     Returns a snapshot descriptor of a current state of the <see cref="Doc" /> associated to this
    ///     <see cref="Transaction" />.
    /// </summary>
    /// <remarks>
    ///     This snapshot information can be then used to encode <see cref="Doc" /> state at a particular point in time
    ///     with <see cref="EncodeStateFromSnapshotV1" /> or <see cref="EncodeStateFromSnapshotV2" />.
    /// </remarks>
    /// <returns>
    ///     A snapshot descriptor of a current state of the <see cref="Doc" /> associated to this <see cref="Transaction" />.
    /// </returns>
    public byte[] Snapshot()
    {
        var handle = TransactionChannel.Snapshot(Handle, out var length);

        return MemoryReader.ReadAndDestroyBytes(handle.Checked(), length);
    }

    /// <summary>
    ///     Encodes the state of the <see cref="Doc" /> associated to this <see cref="Transaction" /> at a point in time
    ///     specified by the provided <paramref name="snapshot" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This function requires a <see cref="Doc" /> with <see cref="DocOptions.SkipGarbageCollection" /> set to
    ///         <c>true</c>, otherwise "time travel" would not be a safe operation). If this is not respected,
    ///         <c>null</c> will be returned.
    ///     </para>
    ///     <para>
    ///         The <paramref name="snapshot" /> is generated by <see cref="Snapshot" />. This is useful to generate a past
    ///         view
    ///         of the document.
    ///     </para>
    ///     <para>
    ///         The returned update is binary compatible with <see cref="ApplyV1" />.
    ///     </para>
    /// </remarks>
    /// <param name="snapshot">
    ///     The snapshot to be used to encode the <see cref="Doc" /> state.
    /// </param>
    /// <returns>
    ///     The state difference update that can be applied <see cref="ApplyV1" /> to return the document to the state when the
    ///     <paramref name="snapshot" /> was created.
    /// </returns>
    public byte[]? EncodeStateFromSnapshotV1(byte[] snapshot)
    {
        var handle = TransactionChannel.EncodeStateFromSnapshotV1(
            Handle,
            snapshot,
            (uint)snapshot.Length,
            out var length);

        return handle != nint.Zero ? MemoryReader.ReadAndDestroyBytes(handle, length) : null;
    }

    /// <summary>
    ///     Encodes the state of the <see cref="Doc" /> associated to this <see cref="Transaction" /> at a point in time
    ///     specified by the provided <paramref name="snapshot" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This function requires a <see cref="Doc" /> with <see cref="DocOptions.SkipGarbageCollection" /> set to
    ///         <c>true</c>, otherwise "time travel" would not be a safe operation). If this is not respected,
    ///         <c>null</c> will be returned.
    ///     </para>
    ///     <para>
    ///         The <paramref name="snapshot" /> is generated by <see cref="Snapshot" />. This is useful to generate a past
    ///         view
    ///         of the document.
    ///     </para>
    ///     <para>
    ///         The returned update is binary compatible with <see cref="ApplyV2" />.
    ///     </para>
    /// </remarks>
    /// <param name="snapshot">
    ///     The snapshot to be used to encode the <see cref="Doc" /> state.
    /// </param>
    /// <returns>
    ///     The state difference update that can be applied <see cref="ApplyV2" /> to return the document to the state when the
    ///     <paramref name="snapshot" /> was created.
    /// </returns>
    public byte[]? EncodeStateFromSnapshotV2(byte[] snapshot)
    {
        var handle = TransactionChannel.EncodeStateFromSnapshotV2(
            Handle,
            snapshot,
            (uint)snapshot.Length,
            out var length);

        return handle != nint.Zero ? MemoryReader.ReadAndDestroyBytes(handle, length) : null;
    }

    /// <summary>
    ///     Returns the <see cref="Array" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </summary>
    /// <param name="name">The name of the <see cref="Array" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="Array" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </returns>
    public Array? GetArray(string name)
    {
        var handle = GetWithKind(name, BranchKind.Array);

        return handle != nint.Zero ? doc.GetArray(handle, isDeleted: false) : null;
    }

    /// <summary>
    ///     Returns the <see cref="Map" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </summary>
    /// <param name="name">The name of the <see cref="Map" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="Map" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </returns>
    public Map? GetMap(string name)
    {
        var handle = GetWithKind(name, BranchKind.Map);

        return handle != nint.Zero ? doc.GetMap(handle, isDeleted: false) : null;
    }

    /// <summary>
    ///     Returns the <see cref="Text" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </summary>
    /// <param name="name">The name of the <see cref="Text" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="Text" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </returns>
    public Text? GetText(string name)
    {
        var handle = GetWithKind(name, BranchKind.Text);

        return handle != nint.Zero ? doc.GetText(handle, isDeleted: false) : null;
    }

    /// <summary>
    ///     Returns the <see cref="XmlElement" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />,
    ///     or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </summary>
    /// <param name="name">The name of the <see cref="XmlElement" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="XmlElement" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </returns>
    public XmlElement? GetXmlElement(string name)
    {
        var handle = GetWithKind(name, BranchKind.XmlElement);

        return handle != nint.Zero ? doc.GetXmlElement(handle, isDeleted: false) : null;
    }

    /// <summary>
    ///     Returns the <see cref="XmlText" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </summary>
    /// <param name="name">The name of the <see cref="XmlText" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="XmlText" /> at the <see cref="Doc" /> root level, identified by <paramref name="name" />, or
    ///     <c>null</c> if no entry was defined under <paramref name="name" /> before.
    /// </returns>
    public XmlText? GetXmlText(string name)
    {
        var handle = GetWithKind(name, BranchKind.XmlText);

        return handle != nint.Zero ? doc.GetXmlText(handle, isDeleted: false) : null;
    }

    private nint GetWithKind(string name, BranchKind expectedKind)
    {
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var branchHandle = TransactionChannel.Get(Handle, unsafeName.Handle);
        if (branchHandle == nint.Zero)
        {
            return nint.Zero;
        }

        var branchKind = (BranchKind)BranchChannel.Kind(branchHandle);

        if (branchKind == BranchKind.Null)
        {
            return nint.Zero;
        }

        if (branchKind != expectedKind)
        {
            throw new YDotNetException($"Expected '{expectedKind}', got '{branchKind}'.");
        }

        return branchHandle;
    }
}
