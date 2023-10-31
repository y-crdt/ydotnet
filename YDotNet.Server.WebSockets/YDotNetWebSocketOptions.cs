using Microsoft.AspNetCore.Http;

namespace YDotNet.Server.WebSockets;

public delegate Task AuthDelegate(HttpContext httpContext, DocumentContext context);

public sealed class YDotNetWebSocketOptions
{
    public AuthDelegate? OnAuthenticateAsync { get; set; }
}
