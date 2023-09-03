using YDotNet.Document.Events;
using YDotNet.Document.Options;
using YDotNet.Document.Transactions;
using YDotNet.Document.Types.Maps;
using YDotNet.Document.Types.Texts;
using YDotNet.Document.Types.XmlElements;
using YDotNet.Document.Types.XmlTexts;
using YDotNet.Document.UndoManagers;
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
public class Doc : IDisposable
{
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
    {
        Handle = DocChannel.NewWithOptions(DocOptionsNative.From(options));
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Doc" /> class with the specified <see cref="Handle" />.
    /// </summary>
    /// <param name="handle">The pointer to be used by this document to manage the native resource.</param>
    internal Doc(nint handle)
    {
        Handle = handle;
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
    internal nint Handle { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        DocChannel.Destroy(Handle);
    }

    /// <summary>
    ///     Creates a copy of the current <see cref="Doc" /> instance.
    /// </summary>
    /// <remarks>The instance returned will not be the same, but they will both control the same document.</remarks>
    /// <returns>A new <see cref="Doc" /> instance that controls the same document.</returns>
    public Doc? Clone()
    {
        return ReferenceAccessor.Access(new Doc(DocChannel.Clone(Handle)));
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="YDotNet.Document.Types.Texts.Text" /> data type instance as a root-level
    ///     type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="YDotNet.Document.Types.Texts.Text" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="YDotNet.Document.Types.Texts.Text" /> instance related to the <c>name</c> provided or
    ///     <c>null</c> if failed.
    /// </returns>
    public Text? Text(string name)
    {
        var nameHandle = MemoryWriter.WriteUtf8String(name);
        var result = ReferenceAccessor.Access(new Text(DocChannel.Text(Handle, nameHandle)));

        MemoryWriter.Release(nameHandle);

        return result;
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="YDotNet.Document.Types.Maps.Map" /> data type instance as a root-level type
    ///     in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="YDotNet.Document.Types.Maps.Map" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="YDotNet.Document.Types.Maps.Map" /> instance related to the <c>name</c> provided or <c>null</c>
    ///     if failed.
    /// </returns>
    public Map? Map(string name)
    {
        var nameHandle = MemoryWriter.WriteUtf8String(name);
        var result = ReferenceAccessor.Access(new Map(DocChannel.Map(Handle, nameHandle)));

        MemoryWriter.Release(nameHandle);

        return result;
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="YDotNet.Document.Types.Arrays.Array" /> data type instance as a root-level
    ///     type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="YDotNet.Document.Types.Arrays.Array" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="YDotNet.Document.Types.Arrays.Array" /> instance related to the <c>name</c> provided or
    ///     <c>null</c> if failed.
    /// </returns>
    public Array? Array(string name)
    {
        var nameHandle = MemoryWriter.WriteUtf8String(name);
        var result = ReferenceAccessor.Access(new Array(DocChannel.Array(Handle, nameHandle)));

        MemoryWriter.Release(nameHandle);

        return result;
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="YDotNet.Document.Types.XmlElements.XmlElement" /> data type instance as a
    ///     root-level type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="YDotNet.Document.Types.XmlElements.XmlElement" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="YDotNet.Document.Types.XmlElements.XmlElement" /> instance related to the <c>name</c> provided
    ///     or <c>null</c> if failed.
    /// </returns>
    public XmlElement? XmlElement(string name)
    {
        // TODO [LSViana] Wrap the XmlElement with an XmlFragment before returning the value.
        var nameHandle = MemoryWriter.WriteUtf8String(name);
        var result = ReferenceAccessor.Access(new XmlElement(DocChannel.XmlElement(Handle, nameHandle)));

        MemoryWriter.Release(nameHandle);

        return result;
    }

    /// <summary>
    ///     Gets or creates a new shared <see cref="YDotNet.Document.Types.XmlTexts.XmlText" /> data type instance as a
    ///     root-level type in this document.
    /// </summary>
    /// <remarks>
    ///     This structure can later be accessed using its <c>name</c>.
    /// </remarks>
    /// <param name="name">The name of the <see cref="YDotNet.Document.Types.XmlTexts.XmlText" /> instance to get.</param>
    /// <returns>
    ///     The <see cref="YDotNet.Document.Types.XmlTexts.XmlText" /> instance related to the <c>name</c> provided
    ///     or <c>null</c> if failed.
    /// </returns>
    public XmlText? XmlText(string name)
    {
        // TODO [LSViana] Wrap the XmlText with an XmlFragment before returning the value.
        var nameHandle = MemoryWriter.WriteUtf8String(name);
        var result = ReferenceAccessor.Access(new XmlText(DocChannel.XmlText(Handle, nameHandle)));

        MemoryWriter.Release(nameHandle);

        return result;
    }

    /// <summary>
    ///     Starts a new read-write <see cref="Transaction" /> on this document.
    /// </summary>
    /// <param name="origin">
    ///     Optional byte marker to indicate the source of changes to be applied by this transaction.
    ///     This value is used by <see cref="UndoManager" />.
    /// </param>
    /// <returns>
    ///     <para>The <see cref="Transaction" /> to perform operations in the document or <c>null</c>.</para>
    ///     <para>
    ///         Returns <c>null</c> if the <see cref="Transaction" /> could not be created because, for example, another
    ///         read-write <see cref="Transaction" /> already exists and was not committed yet.
    ///     </para>
    /// </returns>
    public Transaction? WriteTransaction(byte[]? origin = null)
    {
        var length = (uint) (origin?.Length ?? 0);

        return ReferenceAccessor.Access(
            new Transaction(DocChannel.WriteTransaction(Handle, length, origin)));
    }

    /// <summary>
    ///     Starts a new read-only <see cref="Transaction" /> on this document.
    /// </summary>
    /// <returns>
    ///     <para>The <see cref="Transaction" /> to perform operations in the document or <c>null</c>.</para>
    ///     <para>
    ///         Returns <c>null</c> if the <see cref="Transaction" /> could not be created because, for example, another
    ///         read-write <see cref="Transaction" /> already exists and was not committed yet.
    ///     </para>
    /// </returns>
    public Transaction? ReadTransaction()
    {
        return ReferenceAccessor.Access(new Transaction(DocChannel.ReadTransaction(Handle)));
    }

    /// <summary>
    ///     Destroys the current document, sending a <c>destroy</c> event and
    ///     clearing up all the registered callbacks.
    /// </summary>
    public void Clear()
    {
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
    public EventSubscription ObserveClear(Action<ClearEvent> action)
    {
        var subscriptionId = DocChannel.ObserveClear(
            Handle,
            nint.Zero,
            (_, doc) => action(ClearEventNative.From(new Doc(doc)).ToClearEvent()));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance,
    ///     for the <see cref="Clear" /> method.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveClear(EventSubscription subscription)
    {
        DocChannel.UnobserveClear(Handle, subscription.Id);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <remarks>
    ///     The updates are encoded using <c>lib0</c> V1 encoding and they can be  passed to remote peers right away.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription ObserveUpdatesV1(Action<UpdateEvent> action)
    {
        var subscriptionId = DocChannel.ObserveUpdatesV1(
            Handle,
            nint.Zero,
            (_, length, data) => action(UpdateEventNative.From(length, data).ToUpdateEvent()));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveUpdatesV1(EventSubscription subscription)
    {
        DocChannel.UnobserveUpdatesV1(Handle, subscription.Id);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <remarks>
    ///     The updates are encoded using <c>lib0</c> V1 encoding and they can be  passed to remote peers right away.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription ObserveUpdatesV2(Action<UpdateEvent> action)
    {
        var subscriptionId = DocChannel.ObserveUpdatesV2(
            Handle,
            nint.Zero,
            (_, length, data) => action(UpdateEventNative.From(length, data).ToUpdateEvent()));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveUpdatesV2(EventSubscription subscription)
    {
        DocChannel.UnobserveUpdatesV2(Handle, subscription.Id);
    }

    /// <summary>
    ///     Subscribes a callback function for changes performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <remarks>
    ///     The updates are encoded using <c>lib0</c> V1 encoding and they can be  passed to remote peers right away.
    /// </remarks>
    /// <param name="action">The callback to be executed when a <see cref="Transaction" /> is committed.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription ObserveAfterTransaction(Action<AfterTransactionEvent> action)
    {
        var subscriptionId = DocChannel.ObserveAfterTransaction(
            Handle,
            nint.Zero,
            (_, afterTransactionEvent) => action(afterTransactionEvent.ToAfterTransactionEvent()));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed within <see cref="Transaction" /> scope.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveAfterTransaction(EventSubscription subscription)
    {
        DocChannel.UnobserveAfterTransaction(Handle, subscription.Id);
    }

    /// <summary>
    ///     Subscribes a callback function for changes in the sub-documents.
    /// </summary>
    /// <param name="action">The callback to be executed when a sub-document changes.</param>
    /// <returns>The subscription for the event. It may be used to unsubscribe later.</returns>
    public EventSubscription ObserveSubDocs(Action<SubDocsEvent> action)
    {
        var subscriptionId = DocChannel.ObserveSubDocs(
            Handle,
            nint.Zero,
            (_, subDocsEvent) => action(subDocsEvent.ToSubDocsEvent()));

        return new EventSubscription(subscriptionId);
    }

    /// <summary>
    ///     Unsubscribes a callback function, represented by an <see cref="EventSubscription" /> instance, for changes
    ///     performed in the sub-documents.
    /// </summary>
    /// <param name="subscription">The subscription that represents the callback function to be unobserved.</param>
    public void UnobserveSubDocs(EventSubscription subscription)
    {
        DocChannel.UnobserveSubDocs(Handle, subscription.Id);
    }
}
