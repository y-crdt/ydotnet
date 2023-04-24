using YDotNet.Native;

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
    ///     Gets the unique client identifier of the document.
    /// </summary>
    public ulong Id => DocChannel.Id(Handle);

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
