using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace YDotNet.Server.WebSockets;

public sealed class YDotNetActionResult(string documentName) : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        var middleware = context.HttpContext.RequestServices.GetRequiredService<YDotNetSocketMiddleware>();

        await middleware.InvokeAsync(context.HttpContext, documentName).ConfigureAwait(false);
    }
}
