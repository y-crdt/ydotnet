using Microsoft.AspNetCore.Builder;
using YDotNet.Server.WebSockets;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddWebSockets(this YDotnetRegistration registration)
    {
        registration.Services.AddSingleton<YDotNetSocketMiddleware>();
        return registration;
    }

    public static void UseYDotnetWebSockets(this IApplicationBuilder app)
    {
        var middleware = app.ApplicationServices.GetRequiredService<YDotNetSocketMiddleware>();

        app.Run(middleware.InvokeAsync);
    }
}
