using Microsoft.AspNetCore.Http;

namespace YDotNet.Server.WebSockets;

#pragma warning disable MA0048 // File name must match type name
public delegate Task AuthDelegate(HttpContext httpContext, DocumentContext context);
#pragma warning restore MA0048 // File name must match type name

public sealed class YDotNetWebSocketOptions
{
    public AuthDelegate? OnAuthenticateAsync { get; set; }
}
