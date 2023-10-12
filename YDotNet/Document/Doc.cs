using System.Reflection.Metadata;
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
    private readonly EventSubscriptions subscriptions = new();
    private readonly bool isCloned;

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
        : base(CreateDoc(options))
    {
    }

    internal Doc(nint handle, bool isCloned)
        : base(handle)
    {
        this.isCloned = isCloned;
    }

    private static nint CreateDoc(DocOptions options)
    {
        var unsafeOptions = DocOptionsNative.From(options);

        return DocChannel.NewWithOptions(unsafeOptions);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Doc"/> class.
    /// </summary>
    ~Doc()
    {
        Dispose(false);
    }

    /// <inheritdoc/>
    protected internal override void DisposeCore(bool disposing)
    {
        if (isCloned)
        {
            DocChannel.Destroy(Handle);
        }
    }

    /// <summary>
    ///     Gets the unique client identifier of this <see cref="Doc" /> instance.
    /// </summary>
    public ulong Id => DocChannel.Id(Handle);

    /// <summary>
    ///     Gets the unique document identifier of this <see cref="Doc" /> instance.
    /// </summary>
    public string Guid => MemoryReader.ReadUtf8String(DocChannel.Guid(Handle));

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
            MemoryReader.TryReadUtf8String(DocChannel.CollectionId(Handle), out var result);
            return result;
        }
    }

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Doc" /> instance requested a data load.
    /// </summary>
    /// <remarks>
    ///     This flag is often used by the parent <see cref="Doc" /> instance to check if this <see cref="Doc" /> instance requested a data load.
    /// </remarks>
    public bool ShouldLoad => DocChannel.ShouldLoad(Handle);

    /// <summary>
    ///     Gets a value indicating whether this <see cref="Doc" /> instance will auto load.
    /// </summary>
    /// <remarks>
    ///     Auto loaded nested <see cref="Doc" /> instances automatically send a load request to their parent <see cref="Doc" /> instances.
    /// </remarks>
    public bool AutoLoad => DocChannel.AutoLoad(Handle);

    /// <summary>
    ///     Creates a copy of the current <see cref="Doc" /> instance.
    /// </summary>
    /// <remarks>The instance returned will not be the same, but they will both control the same document.</remarks>
    /// <returns>A new <see cref="Doc" /> instance that controls the same document.</returns>
    public Doc Clone()
    {
        var handle = DocChannel.Clone(Handle).Checked();

        return new Doc(handle, true);
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
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = DocChannel.Text(Handle, unsafeName.Handle);

        return new Text(handle.Checked());
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
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = DocChannel.Map(Handle, unsafeName.Handle);

        return new Map(handle.Checked());
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
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = DocChannel.Array(Handle, unsafeName.Handle);

        return new Array(handle.Checked());
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
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = DocChannel.XmlElement(Handle, unsafeName.Handle);

        return new XmlElement(handle.Checked());
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
        using var unsafeName = MemoryWriter.WriteUtf8String(name);

        var handle = DocChannel.XmlText(Handle, unsafeName.Handle);

        return new XmlText(handle.Checked());
    }

    /// <summary>
    ///     Starts a new read-write <see cref="Transaction" /> on this document.
    /// </summary>
    /// <param name="origin">Optional byte marker to indicate the source of changes to be applied by this transaction.</param>
    /// <returns>The <see cref="Transaction" /> to perform operations in the document.</returns>
    /// <exception cref="YDotNetException">Another exception is pending.</exception>
    public Transaction WriteTransaction(byte[]? origin = null)
    {
        var handle = DocChannel.WriteTransaction(Handle, (uint)(origin?.Length ?? 0), origin);

        if (handle == nint.Zero)
        {
            ThrowHelper.PendingTransaction();
            return default!;
        }

        return new Transaction(handle);
    }

    /// <summary>
    ///     Starts a new read-only <see cref="Transaction" /> on this document.
    /// </summary>
    /// <returns>The <see cref="Transaction" /> to perform operations in the document.</returns>
    /// <exception cref="YDotNetException">Another exception is pending.</exception>
    public Transaction ReadTransaction()
    {
        var handle = DocChannel.ReadTransaction(Handle);

        if (handle == nint.Zero)
        {
            ThrowHelper.PendingTransaction();
            return default!;
        }

        return new Transaction(handle);
    }

    /// <summary>
    ///     Destroys the current document, sending a <c>destroy</c> event and clearing up all the registered callbacks.
    /// </summary>
    public void Clear()
    {
        subscriptions.Clear();

        DocChannel.Clear(Handle);
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
        DocChannel.Load(Handle, transaction.Handle);
    }

    /// <summary>
    ///     Subscribes a callback function to be called when the <see cref="Clear" /> method is called.
    /// </summary>
    /// <param name="action">The callback function.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveClear(Action<ClearEvent> action)
    {
        DocChannel.ObserveClearCallback callback = (_, doc) => action(ClearEventNative.From(new Doc(doc, false)).ToClearEvent());

        var subscriptionId = DocChannel.ObserveClear(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            DocChannel.UnobserveClear(Handle, subscriptionId);
        });
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
        DocChannel.ObserveUpdatesCallback callback = (_, length, data) => action(UpdateEventNative.From(length, data).ToUpdateEvent());

        var subscriptionId = DocChannel.ObserveUpdatesV1(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            DocChannel.UnobserveUpdatesV1(Handle, subscriptionId);
        });
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
        DocChannel.ObserveUpdatesCallback callback = (_, length, data) => action(UpdateEventNative.From(length, data).ToUpdateEvent());

        var subscriptionId = DocChannel.ObserveUpdatesV2(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            DocChannel.UnobserveUpdatesV2(Handle, subscriptionId);
        });
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
        DocChannel.ObserveAfterTransactionCallback callback = (_, eventHandler) => action(MemoryReader.ReadStruct<AfterTransactionEventNative>(eventHandler).ToAfterTransactionEvent());

        var subscriptionId = DocChannel.ObserveAfterTransaction(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            DocChannel.UnobserveAfterTransaction(Handle, subscriptionId);
        });
    }

    /// <summary>
    ///     Subscribes a callback function for changes in the sub-documents.
    /// </summary>
    /// <param name="action">The callback to be executed when a sub-document changes.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public IDisposable ObserveSubDocs(Action<SubDocsEvent> action)
    {
        DocChannel.ObserveSubdocsCallback callback = (_, eventHandle) => action(MemoryReader.ReadStruct<SubDocsEventNative>(eventHandle).ToSubDocsEvent());

        var subscriptionId = DocChannel.ObserveSubDocs(
            Handle,
            nint.Zero,
            callback);

        return subscriptions.Add(callback, () =>
        {
            DocChannel.UnobserveSubDocs(Handle, subscriptionId);
        });
    }
}
