using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using YDotNet.Server;
using YDotNet.Server.WebSockets;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddWebSockets(this YDotnetRegistration registration, Action<YDotNetWebSocketOptions>? configure = null)
    {
        registration.Services.Configure(configure ?? (x => { }));
        registration.Services.AddSingleton<YDotNetSocketMiddleware>();

        registration.Services.AddSingleton<IDocumentCallback>(x =>
            x.GetRequiredService<YDotNetSocketMiddleware>());

        return registration;
    }

    public static void UseYDotnetWebSockets(this IApplicationBuilder app)
    {
        var middleware = app.ApplicationServices.GetRequiredService<YDotNetSocketMiddleware>();

        app.Run(middleware.InvokeAsync);
    }
}
