namespace YDotNet.Tests.Server.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using NUnit.Framework;
using Testcontainers.MongoDb;
using YDotNet.Server.MongoDB;
using YDotNet.Server.Storage;

[TestFixture]
[Category("Docker")]
public class MongoDocumentStorageFixture : DocumentStorageTests
{
#pragma warning disable NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private readonly MongoDbContainer mongo = new MongoDbBuilder().Build();
#pragma warning restore NUnit1032 // An IDisposable field/property should be Disposed in a TearDown method
    private IServiceProvider services;

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await mongo.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        await mongo.StopAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        services = new ServiceCollection()
            .AddSingleton<IMongoClient>(_ => new MongoClient(mongo.GetConnectionString()))
            .AddLogging()
            .AddYDotNet()
            .AddMongoStorage()
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

        if (services is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    protected override IDocumentStorage CreateSut()
    {
        return services.GetRequiredService<IDocumentStorage>();
    }
}
