using YDotNet.Document;

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable SA1649 // File name should match first type name

namespace YDotNet.Server;

public abstract class DocumentEvent
{
    required public IDocumentManager Source { get; init; }

    required public DocumentContext Context { get; init; }
}

public class DocumentChangeEvent : DocumentEvent
{
    required public Doc Document { get; init; }
}

public class DocumentLoadEvent : DocumentEvent
{
    required public Doc Document { get; init; }
}

public sealed class DocumentChangedEvent : DocumentChangeEvent
{
    required public byte[] Diff { get; init; }
}

public sealed class ClientDisconnectedEvent : DocumentEvent
{
    required public DisconnectReason Reason { get; init; }
}

public sealed class ClientAwarenessEvent : DocumentEvent
{
    required public string? ClientState { get; set; }

    required public ulong ClientClock { get; set; }
}

public enum DisconnectReason
{
    Disconnect,
    Cleanup,
}
