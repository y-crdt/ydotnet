namespace YDotNet.Server.MongoDB;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using YDotNet.Server.Storage;

public static class ServiceExtensions
{
    public static YDotnetRegistration AddMongoStorage(this YDotnetRegistration registration, Action<MongoDocumentStorageOptions>? configure = null)
    {
        registration.Services.Configure(configure ?? (x => { }));
        registration.Services.AddSingleton<MongoDocumentStorage>();

        registration.Services.AddSingleton<IDocumentStorage>(
            c => c.GetRequiredService<MongoDocumentStorage>());

        registration.Services.AddSingleton<IHostedService>(
            c => c.GetRequiredService<MongoDocumentStorage>());

        return registration;
    }
}
