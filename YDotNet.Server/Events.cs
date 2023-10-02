using YDotNet.Document;

namespace YDotNet.Server;

public abstract class DocumentEvent
{
    required public IDocumentManager DocumentManager { get; init; }

    required public DocumentContext DocumentContext { get; init; }
}

public class DocumentChangeEvent : DocumentEvent
{
    required public Doc Document { get; init; }
}

public sealed class DocumentChangedEvent : DocumentChangeEvent
{
    required public byte[] Diff { get; init; }
}

public sealed class ClientDisconnectedEvent : DocumentEvent
{
}

public sealed class ClientAwarenessEvent : DocumentEvent
{
    required public string? ClientState { get; set; }

    required public long ClientClock { get; set; }
}
