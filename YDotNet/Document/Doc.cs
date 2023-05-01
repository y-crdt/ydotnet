using YDotNet.Document.Options;
using YDotNet.Native.Doc;

namespace YDotNet.Document;

/// <summary>
///     A Yrs document type.
/// </summary>
/// <remarks>
///     <para>
///         The documents are the most important units of collaborative resources management.
///         All shared collections live within the scope of their corresponding documents. All updates are
///         generated on per document basis (rather than generated per shared type basis). All operations on
///         shared collections happen via <see cref="Transaction" />, whose lifetime is also bound to a document.
///     </para>
///     <para>
///         Document manages the so-called root types, which are top-level shared types definitions (as opposed
///         to recursively nested types).
///     </para>
/// </remarks>
public class Doc : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Doc" /> class.
    /// </summary>
    public Doc()
    {
        Handle = DocChannel.New();
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Doc" /> class with the specified <paramref name="options" />.
    /// </summary>
    /// <param name="options">The options to be used when initializing this document.</param>
    public Doc(DocOptions options)
    {
        Handle = DocChannel.NewWithOptions(DocOptionsNative.From(options));
    }

    /// <summary>
    ///     Gets the unique client identifier of this <see cref="Doc" /> instance.
    /// </summary>
    public ulong Id => DocChannel.Id(Handle);

    /// <summary>
    ///     Gets the unique document identifier of this <see cref="Doc" /> instance.
    /// </summary>
    // TODO [LSViana] Check if this should be of type `Guid`.
    // There's a complication due to string format losing zero to the left in hexadecimal representation.
    public string Guid => DocChannel.Guid(Handle);

    /// <summary>
    ///     Gets the collection identifier of this <see cref="Doc" /> instance.
    /// </summary>
    /// <remarks>
    ///     If none was defined, a <c>null</c> will be returned.
    /// </remarks>
    public string? CollectionId => DocChannel.CollectionId(Handle);

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Doc" /> instance requested a data load.
    /// </summary>
    /// <remarks>
    ///     This flag is often used by the parent <see cref="Doc" /> instance to check if this <see cref="Doc" />
    ///     instance requested a data load.
    /// </remarks>
    public bool ShouldLoad => DocChannel.ShouldLoad(Handle);

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Doc" /> instance will auto load.
    /// </summary>
    /// <remarks>
    ///     Auto loaded nested <see cref="Doc" /> instances automatically send a load request to their parent
    ///     <see cref="Doc" /> instances.
    /// </remarks>
    public bool AutoLoad => DocChannel.AutoLoad(Handle);

    /// <summary>
    ///     Gets the handle to the native resource.
    /// </summary>
    internal nint Handle { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
        DocChannel.Destroy(Handle);

        Handle = default;
    }
}
