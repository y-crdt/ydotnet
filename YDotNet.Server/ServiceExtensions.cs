using Microsoft.Extensions.DependencyInjection.Extensions;
using YDotNet.Server;
using YDotNet.Server.Storage;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static void AddYDotNet(IServiceCollection services)
    {
        services.AddOptions<DocumentManagerOptions>();
        services.TryAddSingleton<IDocumentManager, DefaultDocumentManager>();
        services.TryAddSingleton<IDocumentStorage, InMemoryDocumentStorage>();
    }
}
