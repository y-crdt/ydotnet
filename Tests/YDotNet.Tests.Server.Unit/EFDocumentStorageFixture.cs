namespace YDotNet.Tests.Server.Unit;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Testcontainers.PostgreSql;
using YDotNet.Server.EntityFramework;
using YDotNet.Server.Storage;

[TestFixture]
public class EFDocumentStorageFixture : DocumentStorageTests
{
    private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder().Build();
    private IServiceProvider services;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await postgres.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        await postgres.StopAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        services = new ServiceCollection()
            .AddLogging()
            .AddYDotNet()
            .AddEntityFrameworkStorage(options =>
            {
                options.UseNpgsql(postgres.GetConnectionString());
            })
            .Services
            .BuildServiceProvider();

        foreach (var service in services.GetRequiredService<IEnumerable<IHostedService>>())
        {
            await service.StartAsync(default);
        }
    }

    [TearDown]
    public async Task Teardown()
    {
        foreach (var service in services.GetRequiredService<IEnumerable<IHostedService>>())
        {
            await service.StopAsync(default);
        }
    }

    protected override IDocumentStorage CreateSut()
    {
        return services.GetRequiredService<IDocumentStorage>();
    }
}
