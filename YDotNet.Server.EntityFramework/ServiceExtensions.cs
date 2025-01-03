namespace YDotNet.Server.EntityFramework;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YDotNet.Server.Storage;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddEntityFrameworkStorage(this YDotnetRegistration registration, Action<DbContextOptionsBuilder> build, Action<EFDocumentStorageOptions>? configure = null)
    {
        registration.Services.Configure(configure ?? (x => { }));
        registration.Services.AddSingleton<EFDocumentStorage>();

        registration.Services.AddSingleton<IDocumentStorage>(
            c => c.GetRequiredService<EFDocumentStorage>());

        registration.Services.AddSingleton<IHostedService>(
            c => c.GetRequiredService<EFDocumentStorage>());

        registration.Services.AddSingleton<IHostedService, EFDocumentCleaner>();
        registration.Services.AddDbContextFactory<YDotNetContext>(build);

        return registration;
    }
}
