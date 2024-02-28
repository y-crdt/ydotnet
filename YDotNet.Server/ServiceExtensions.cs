using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using YDotNet.Server;
using YDotNet.Server.Storage;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddYDotNet(this IServiceCollection services)
    {
        services.AddOptions<DocumentManagerOptions>();
        services.TryAddSingleton<IDocumentManager, DefaultDocumentManager>();
        services.TryAddSingleton<IDocumentStorage, InMemoryDocumentStorage>();

        services.AddSingleton<IHostedService>(x =>
            x.GetRequiredService<IDocumentManager>());

        return new YDotnetRegistration
        {
            Services = services,
        };
    }

    public static YDotnetRegistration AddCallback<T>(this YDotnetRegistration registration)
        where T : class, IDocumentCallback
    {
        registration.Services.AddSingleton<IDocumentCallback, T>();
        return registration;
    }

    public static YDotnetRegistration AutoCleanup(this YDotnetRegistration registration, Action<CleanupOptions>? configure = null)
    {
        registration.Services.Configure(configure ?? (x => { }));
        registration.Services.AddSingleton<IHostedService, DefaultDocumentCleaner>();
        return registration;
    }
}

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type
public sealed class YDotnetRegistration
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore MA0048 // File name must match type name
{
    required public IServiceCollection Services { get; init; }
}
