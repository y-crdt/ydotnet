namespace YDotNet.Server.EntityFramework;

using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YDotNet.Server.Storage;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddEntityFrameworkStorage<T>(this YDotnetRegistration registration, Action<EFDocumentStorageOptions>? configure = null)
        where T : DbContext
    {
        registration.Services.Configure(configure ?? (x => { }));
        registration.Services.AddSingleton<EFDocumentStorage<T>>();

        registration.Services.AddSingleton<IDocumentStorage>(
            c => c.GetRequiredService<EFDocumentStorage<T>>());

        registration.Services.AddDbContextFactory<T>();
        registration.Services.AddSingleton<IHostedService, EFDocumentCleaner<T>>();

        return registration;
    }

    public static ModelBuilder UseYDotNet(this ModelBuilder builder)
    {
        builder.Entity<YDotNetDocument>(b =>
        {
            b.ToTable("YDotNetDocument");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .HasMaxLength(1000);
        });

        return builder;
    }
}
