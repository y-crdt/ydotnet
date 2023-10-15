using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace YDotNet.Server.WebSockets;

public sealed class YDotNetActionResult : IActionResult
{
    private readonly string documentName;

    public YDotNetActionResult(string documentName)
    {
        this.documentName = documentName;
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var middleware = context.HttpContext.RequestServices.GetRequiredService<YDotNetSocketMiddleware>();

        await middleware.InvokeAsync(context.HttpContext, this.documentName);
    }
}
