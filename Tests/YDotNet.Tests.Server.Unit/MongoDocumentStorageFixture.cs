namespace YDotNet.Tests.Server.Unit;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using NUnit.Framework;
using Testcontainers.MongoDb;
using YDotNet.Server.MongoDB;
using YDotNet.Server.Storage;

[TestFixture]
public class MongoDocumentStorageFixture : DocumentStorageTests
{
    private readonly MongoDbContainer mongo = new MongoDbBuilder().Build();
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
    }

    protected override IDocumentStorage CreateSut()
    {
        return services.GetRequiredService<IDocumentStorage>();
    }
}
