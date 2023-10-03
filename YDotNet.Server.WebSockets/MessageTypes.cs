namespace YDotNet.Server.WebSockets;

public static class MessageTypes
{
    // Read: https://github.com/yjs/y-websocket/blob/master/src/y-websocket.js#L19
    public const int TypeSync = 0;
    public const int TypeAwareness = 1;
    public const int TypeAuth = 2;
    public const int TypeQueryAwareness = 3;

    // Read: https://github.com/yjs/y-protocols/blob/master/sync.js
    public const int SyncStep1 = 0;
    public const int SyncStep2 = 1;
    public const int SyncUpdate = 2;
}
