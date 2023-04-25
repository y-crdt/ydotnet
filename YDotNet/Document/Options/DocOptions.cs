﻿namespace YDotNet.Document.Options;

/// <summary>
///     Configuration object that, optionally, is used to create <see cref="Doc" /> instances.
/// </summary>
public class DocOptions
{
    /// <summary>
    ///     Gets the globally unique 53-bit integer assigned to the <see cref="Doc" /> replica as its identifier.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If two clients share the same <see cref="Id" /> and will perform any updates, it will result in
    ///         unrecoverable <see cref="Doc" /> state corruption.
    ///     </para>
    ///     <para>
    ///         The same thing may happen if the client restored <see cref="Doc" /> state from snapshot, that didn't
    ///         contain all of that clients updates that were sent to other peers.
    ///     </para>
    /// </remarks>
    public ulong? Id { get; init; }

    /// <summary>
    ///     Gets the globally unique UUID v4 compatible <c>null</c>-terminated string identifier of this <see cref="Doc" />.
    /// </summary>
    /// <remarks>
    ///     If passed as <c>null</c>, a random UUID will be generated instead.
    /// </remarks>
    public Guid? Guid { get; init; }

    /// <summary>
    ///     Gets the UTF-8 encoded, <c>null</c>-terminated string of a collection that this <see cref="Doc" /> belongs to.
    /// </summary>
    /// <remarks>
    ///     It's used only by providers.
    /// </remarks>
    public string? CollectionId { get; init; }

    /// <summary>
    ///     Gets the encoding used by text editing operations on this <see cref="Doc" />.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         It's used to compute <see cref="Text" /> and <see cref="XmlText" /> insertion offsets and string lengths.
    ///     </para>
    ///     <para>
    ///         Read more about the possible values in <see cref="DocEncoding" />.
    ///     </para>
    /// </remarks>
    public DocEncoding? Encoding { get; init; }

    /// <summary>
    ///     Gets the flag that determines whether deleted blocks should be garbage collected during transaction commits.
    /// </summary>
    /// <remarks>
    ///     Setting this value to <c>false</c> means garbage collection will be performed.
    /// </remarks>
    public bool? SkipGarbageCollection { get; init; }

    /// <summary>
    ///     Gets the flag that determines whether subdocuments should be loaded automatically.
    /// </summary>
    /// <remarks>
    ///     If this is a subdocument, remote peers will automatically load the <see cref="Doc" /> as well.
    /// </remarks>
    public bool? AutoLoad { get; init; }

    /// <summary>
    ///     Gets the flag that determines whether the <see cref="Doc" /> should be synchronized by the provider now.
    /// </summary>
    public bool? ShouldLoad { get; init; }
}
