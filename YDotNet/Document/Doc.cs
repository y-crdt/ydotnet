using YDotNet.Document.Events;
using YDotNet.Document.Options;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Infrastructure;
using YDotNet.Native.Document;
using YDotNet.Native.Document.Events;
using Array = YDotNet.Document.Types.Arrays.Array;

namespace YDotNet.Document;

/// <summary>
///     A Yrs document type.
/// </summary>
/// <remarks>
///     <para>
///         The documents are the most important units of collaborative resources management.
///     </para>
///     <para>
///         All shared collections live within the scope of their corresponding documents. All updates are
///         generated on per document basis (rather than generated per shared type basis). All operations on
///         shared collections happen via <see cref="Transaction" />, whose lifetime is also bound to a document.
///     </para>
///     <para>
///         Document manages the so-called root types, which are top-level shared types definitions (as opposed
///         to recursively nested types).
///     </para>
/// </remarks>
public class Doc : UnmanagedResource
{
    private readonly EventSubscriber<AfterTransactionEvent> onAfterTransaction;
    private readonly EventSubscriber<ClearEvent> onClear;
    private readonly EventSubscriber<SubDocsEvent> onSubDocs;
    private readonly EventSubscriber<UpdateEvent> onUpdateV1;
    private readonly EventSubscriber<UpdateEvent> onUpdateV2;
    private readonly Doc? parent;
    private readonly TypeCache typeCache = new();
    private int openTransactions;

    /// <summary>
    ///     Initializes a new instance of the <see cref="Doc" /> class.
    /// </summary>
    /// <remarks>
    ///     The default encoding used for <see cref="Doc" /> instances is see <see cref="DocEncoding.Utf16" />
    ///     to match default encoding used by C#.
    /// </remarks>
    public Doc()
        : this(DocOptions.Default)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Doc" /> class with the specified <paramref name="options" />.
    /// </summary>
    /// <param name="options">The options to be used when initializing this document.</param>
    public Doc(DocOptions options)
        : this(CreateDoc(options), parent: null, isDeleted: false)
    {
    }

    internal Doc(nint handle, Doc? parent, bool isDeleted)
        : base(handle, isDeleted)
    {
        this.parent = parent;

        onClear = new EventSubscriber<ClearEvent>(
            EventManager,
            handle,
            (doc, action) =>
            {
                DocChannel.ObserveClearCallback callback =
                    (_, eventDoc) => action(new ClearEvent(GetDoc(eventDoc, isDeleted: false)));

                return (DocChannel.ObserveClear(doc, nint.Zero, callback), callback);
            },
            (doc, s) => DocChannel.UnobserveClear(doc, s));

        onUpdateV1 = new EventSubscriber<UpdateEvent>(
            EventManager,
            handle,
            (_, action) =>
            {
                DocChannel.ObserveUpdatesCallback callback =
                    (_, length, data) => action(new UpdateEvent(UpdateEventNative.From(length, data)));

                return (DocChannel.ObserveUpdatesV1(Handle, nint.Zero, callback), callback);
            },
            (doc, s) => DocChannel.UnobserveUpdatesV1(doc, s));

        onUpdateV2 = new EventSubscriber<UpdateEvent>(
            EventManager,
            handle,
            (_, action) =>
            {
                DocChannel.ObserveUpdatesCallback callback =
                    (_, length, data) => action(new UpdateEvent(UpdateEventNative.From(length, data)));

                return (DocChannel.ObserveUpdatesV2(Handle, nint.Zero, callback), callback);
            },
            (doc, s) => DocChannel.UnobserveUpdatesV2(doc, s));

        onAfterTransaction = new EventSubscriber<AfterTransactionEvent>(
            EventManager,
            handle,
            (doc, action) =>
            {
                DocChannel.ObserveAfterTransactionCallback callback =
                    (_, ev) => action(
                        new AfterTransactionEvent(MemoryReader.ReadStruct<AfterTransactionEventNative>(ev)));

                return (DocChannel.ObserveAfterTransaction(doc, nint.Zero, callback), callback);
            },
            (doc, s) => DocChannel.UnobserveAfterTransaction(doc, s));

        onSubDocs = new EventSubscriber<SubDocsEvent>(
            EventManager,
            handle,
            (doc, action) =>
            {
                DocChannel.ObserveSubdocsCallback callback =
                    (_, ev) => action(new SubDocsEvent(MemoryReader.ReadStruct<SubDocsEventNative>(ev), this));

                return (DocChannel.ObserveSubDocs(doc, nint.Zero, callback), callback);
            },
            (doc, s) => DocChannel.UnobserveSubDocs(doc, s));
    }

    /// <summary>
    ///     Gets the unique client identifier of this <see cref="Doc" /> instance.
    /// </summary>
    public ulong Id
    {
        get
        {
            ThrowIfDisposed();

            return DocChannel.Id(Handle);
        }
    }

    /// <summary>
    ///     Gets the unique document identifier of this <see cref="Doc" /> instance.
    /// </summary>
    public string Guid
    {
        get
        {
            ThrowIfDisposed();

            return MemoryReader.ReadUtf8String(DocChannel.Guid(Handle));
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Doc" /> instance requested a data load.
    /// </summary>
    /// <remarks>
    ///     This flag is often used by the parent <see cref="Doc" /> instance to check if this <see cref="Doc" /> instance
    ///     requested a data load.
    /// </remarks>
    public bool ShouldLoad
    {
        get
        {
            ThrowIfDisposed();

            return DocChannel.ShouldLoad(Handle);
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Doc" /> instance will auto load.
    /// </summary>
    /// <remarks>
    ///     Auto loaded nested <see cref="Doc" /> instances automatically send a load request to their parent
    ///     <see cref="Doc" /> instances.
    /// </remarks>
    public bool AutoLoad
    {
        get
        {
            ThrowIfDisposed();

            return DocChannel.AutoLoad(Handle);
        }
    }

    /// <summary>
    ///     Gets the collection identifier of this <see cref="Doc" /> instance.
    /// </summary>
    /// <remarks>
    ///     If none was defined, a <c>null</c> will be returned.
    /// </remarks>
    public string? CollectionId
    {
        get
        {
            ThrowIfDisposed();

            MemoryReader.TryReadUtf8String(DocChannel.CollectionId(Handle), out var result);
            return result;
        }
    }

    internal EventManager EventManager { get; } = new();

    /// <summary>
    ///     Finalizes an instance of the <see cref="Doc" /> class.
    /// </summary>
    ~Doc()
    {
        Dispose(disposing: false);
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="Types.Texts.Text" /> data type instance as a root-level
    ///     type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="Types.Texts.Text" /> instance to get.</param>
    /// <returns>The <see cref="Types.Texts.Text" /> instance related to the <c>name</c> provided.</returns>
    public Text Text(string name)
    {
        ThrowIfDisposed();
        ThrowIfOpenTransaction();

        using var unsafeName = MemoryWriter.WriteUtf8String(name);
        var handle = DocChannel.Text(Handle, unsafeName.Handle);

        return GetText(handle, isDeleted: false);
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="Types.Maps.Map" /> data type instance as a root-level type
    ///     in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="Types.Maps.Map" /> instance to get.</param>
    /// <returns>The <see cref="Types.Maps.Map" /> instance related to the <c>name</c> provided.</returns>
    public Map Map(string name)
    {
        ThrowIfDisposed();
        ThrowIfOpenTransaction();

        using var unsafeName = MemoryWriter.WriteUtf8String(name);
        var handle = DocChannel.Map(Handle, unsafeName.Handle);

        return GetMap(handle, isDeleted: false);
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="Types.Arrays.Array" /> data type instance as a root-level
    ///     type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="Types.Arrays.Array" /> instance to get.</param>
    /// <returns>The <see cref="Types.Arrays.Array" /> instance related to the <c>name</c> provided.</returns>
    public Array Array(string name)
    {
        ThrowIfDisposed();
        ThrowIfOpenTransaction();

        using var unsafeName = MemoryWriter.WriteUtf8String(name);
        var handle = DocChannel.Array(Handle, unsafeName.Handle);

        return GetArray(handle, isDeleted: false);
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="Types.XmlElements.XmlElement" /> data type instance as a
    ///     root-level type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="Types.XmlElements.XmlElement" /> instance to get.</param>
    /// <returns>The <see cref="Types.XmlElements.XmlElement" /> instance related to the <c>name</c> provided.</returns>
    public XmlElement XmlElement(string name)
    {
        ThrowIfDisposed();
        ThrowIfOpenTransaction();

        using var unsafeName = MemoryWriter.WriteUtf8String(name);
        var handle = DocChannel.XmlElement(Handle, unsafeName.Handle);

        return GetXmlElement(handle, isDeleted: false);
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="Types.XmlTexts.XmlText" /> data type instance as a
    ///     root-level type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="Types.XmlTexts.XmlText" /> instance to get.</param>
    /// <returns>The <see cref="Types.XmlTexts.XmlText" /> instance related to the <c>name</c> provided.</returns>
    public XmlText XmlText(string name)
    {
        ThrowIfDisposed();
        ThrowIfOpenTransaction();

        using var unsafeName = MemoryWriter.WriteUtf8String(name);
        var handle = DocChannel.XmlText(Handle, unsafeName.Handle);

        return GetXmlText(handle, isDeleted: false);
    }

    /// <summary>
    ///     Starts a new read-write <see cref="Transaction" /> on this document.
    /// </summary>
    /// <param name="origin">Optional byte marker to indicate the source of changes to be applied by this transaction.</param>
    /// <returns>The <see cref="Transaction" /> to perform write operations in the document.</returns>
    /// <exception cref="YDotNetException">Another write transaction has been created and not commited yet.</exception>
    public Transaction WriteTransaction(byte[]? origin = null)
    {
        ThrowIfDisposed();

        var handle = DocChannel.WriteTransaction(Handle, (uint) (origin?.Length ?? 0), origin);

        if (handle == nint.Zero)
        {
            ThrowHelper.PendingTransaction();
            return default!;
        }

        return new Transaction(handle, this);
    }

    /// <summary>
    ///     Starts a new read-only <see cref="Transaction" /> on this document.
    /// </summary>
    /// <returns>The <see cref="Transaction" /> to perform read operations in the document.</returns>
    /// <exception cref="YDotNetException">Another write transaction has been created and not commited yet.</exception>
    public Transaction ReadTransaction()
    {
        ThrowIfDisposed();

        var handle = DocChannel.ReadTransaction(Handle);

        if (handle == nint.Zero)
        {
            ThrowHelper.PendingTransaction();
            return default!;
        }

        return new Transaction(handle, this);
    }

    /// <summary>
    ///     Destroys the current document, sending a <c>destroy</c> event and clearing up all the registered callbacks.
    /// </summary>
    public void Clear()
    {
        ThrowIfDisposed();

        DocChannel.Clear(Handle);

        onClear.Clear();
        onUpdateV1.Clear();
        onUpdateV2.Clear();
        onAfterTransaction.Clear();
        onSubDocs.Clear();
    }

    /// <summary>
    ///     Manually send a load request to the parent document of this sub-document.
    /// </summary>
    /// <remarks>
    ///     Works only if current document is a sub-document of an another document.
    /// </remarks>
    /// <param name="transaction">A read-only <see cref="Transaction" /> of the parent document.</param>
    public void Load(Transaction transaction)
    {
        ThrowIfDisposed();

        DocChannel.Load(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Subscribes a callback function to be called when the <see cref="Clear" /> method is called.
    /// </summary>
    /// <param name="action">The callback function.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveClear(Action<ClearEvent> action)
    {
        ThrowIfDisposed();

        return onClear.Subscribe(action);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <remarks>
    ///     The updates are encoded using <c>lib0</c> V1 encoding and they can be  passed to remote peers right away.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveUpdatesV1(Action<UpdateEvent> action)
    {
        ThrowIfDisposed();

        return onUpdateV1.Subscribe(action);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <remarks>
    ///     The updates are encoded using <c>lib0</c> V1 encoding and they can be  passed to remote peers right away.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveUpdatesV2(Action<UpdateEvent> action)
    {
        ThrowIfDisposed();

        return onUpdateV2.Subscribe(action);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <remarks>
    ///     The updates are encoded using <c>lib0</c> V1 encoding and they can be  passed to remote peers right away.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveAfterTransaction(Action<AfterTransactionEvent> action)
    {
        ThrowIfDisposed();

        return onAfterTransaction.Subscribe(action);
    }

    /// <summary>
    ///     Subscribes a callback function for changes in the sub-documents.
    /// </summary>
    /// <param name="action">The callback to be executed when a sub-document changes.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveSubDocs(Action<SubDocsEvent> action)
    {
        ThrowIfDisposed();

        return onSubDocs.Subscribe(action);
    }

    internal Doc GetDoc(nint handle, bool isDeleted)
    {
        if (handle == Handle)
        {
            return this;
        }

        if (!isDeleted)
        {
            // Prevent the sub-document to be released while we are working with it.
            handle = DocChannel.Clone(handle);
        }

        return GetOrAdd(handle, (_, doc) => new Doc(handle, doc, isDeleted));
    }

    // TODO This is a temporary solution to track the amount of transactions a document has open.
    // It's fragile because a cloned instance of the same document won't be synchronized and if a `Transaction`
    // is opened in the cloned instance, it'll receive `null` from the Rust side and will cause the
    // `ThrowHelper.PendingTransaction()` to run (which is acceptable since it's a managed exception).
    internal void NotifyTransactionStarted()
    {
        openTransactions++;
    }

    internal void NotifyTransactionClosed()
    {
        openTransactions--;
    }

    internal Map GetMap(nint handle, bool isDeleted)
    {
        return GetOrAdd(handle, (h, doc) => new Map(h, doc, isDeleted));
    }

    internal Array GetArray(nint handle, bool isDeleted)
    {
        return GetOrAdd(handle, (h, doc) => new Array(h, doc, isDeleted));
    }

    internal Text GetText(nint handle, bool isDeleted)
    {
        return GetOrAdd(handle, (h, doc) => new Text(h, doc, isDeleted));
    }

    internal XmlText GetXmlText(nint handle, bool isDeleted)
    {
        return GetOrAdd(handle, (h, doc) => new XmlText(h, doc, isDeleted));
    }

    internal XmlElement GetXmlElement(nint handle, bool isDeleted)
    {
        return GetOrAdd(handle, (h, doc) => new XmlElement(h, doc, isDeleted));
    }

    /// <inheritdoc />
    protected override void DisposeCore(bool disposing)
    {
        if (IsDisposed)
        {
            return;
        }

        MarkDisposed();

        if (disposing)
        {
            // Clears all active subscriptions that have not been closed yet.
            EventManager.Clear();
        }

        DocChannel.Destroy(Handle);
    }

    private T GetOrAdd<T>(nint handle, Func<nint, Doc, T> factory)
        where T : UnmanagedResource
    {
        var doc = GetRootDoc();

        return doc.typeCache.GetOrAdd(handle, h => factory(h, doc));
    }

    private void ThrowIfOpenTransaction()
    {
        if (openTransactions > 0)
        {
            ThrowHelper.PendingTransaction();
        }
    }

    private Doc GetRootDoc()
    {
        return parent?.GetRootDoc() ?? this;
    }

    private static nint CreateDoc(DocOptions options)
    {
        return DocChannel.NewWithOptions(options.ToNative());
    }
}
