#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1649 // File name should match first type name
#pragma warning disable SA1201 // Elements should appear in the correct order

namespace YDotNet.Protocol;

/// <summary>
/// Base class for all messages.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-websocket/blob/master/src/y-websocket.js#L19.
/// </remarks>
public abstract record BaseMessage;

/// <summary>
/// Represents an unknown message.
/// </summary>
public record UnknownMessage(ulong Identifier) : BaseMessage;

/// <summary>
/// Response from client or server to provide the current awareness information.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/awareness.js.
/// </remarks>
public record QueryAwarenessMessage : BaseMessage
{
    /// <summary>
    /// Identifier for the this message.
    /// </summary>
    public const long Identifier = 1;
}

/// <summary>
/// Auth request from client to server.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/auth.js.
/// </remarks>
public record AuthErrorMessage(string Reason) : BaseMessage
{
    /// <summary>
    /// Identifier for the this message.
    /// </summary>
    public const long Identifier = 2;
}

/// <summary>
/// Synchronization messages from client or server.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/sync.js.
/// </remarks>
public abstract record SyncMessage : BaseMessage
{
    /// <summary>
    /// The base identifier for all sync messages.
    /// </summary>
    public const long BaseIdentifier = 0;
}

/// <summary>
/// Represents an unknown message.
/// </summary>
public record UnknownSyncMessage(ulong Identifier) : SyncMessage;

/// <summary>
/// Step 1: Includes the State Set of the sending client. When received, the client should reply with Step 2.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/sync.js.
/// </remarks>
public record SyncStep1Message(byte[] StateVector) : SyncMessage
{
    /// <summary>
    /// Identifier for the this message.
    /// </summary>
    public const long Identifier = 0;
}

/// <summary>
/// Step 2: Includes all missing structs and the complete delete set. When received, the client is assured that it received all information from the remote client.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/sync.js.
/// </remarks>
public record SyncStep2Message(byte[] Update) : SyncMessage
{
    /// <summary>
    /// Identifier for the this message.
    /// </summary>
    public const long Identifier = 1;
}

/// <summary>
/// Update step to send differential changes to the other peer.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/sync.js#L40.
/// </remarks>
public record SyncUpdateMessage(byte[] Update) : SyncMessage
{
    /// <summary>
    /// Identifier for the this message.
    /// </summary>
    public const long Identifier = 2;
}

/// <summary>
/// Response from client or server to provide the current awareness information.
/// </summary>
/// <remarks>
/// See: https://github.com/yjs/y-protocols/blob/master/awareness.js.
/// </remarks>
public record AwarenessMessage(params AwarenessInformation[] Clients) : BaseMessage
{
    /// <summary>
    /// Identifier for the awareness message.
    /// </summary>
    public const long Identifier = 1;
}

/// <summary>
/// Represents a single client in th awareness message.
/// </summary>
/// <param name="ClientId">The client ID.</param>
/// <param name="Clock">The update clock.</param>
/// <param name="State">The state of the client to store abitrary data.</param>
public record struct AwarenessInformation(ulong ClientId, ulong Clock, string? State);
