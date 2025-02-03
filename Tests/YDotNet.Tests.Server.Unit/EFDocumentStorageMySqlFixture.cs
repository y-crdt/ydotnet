namespace YDotNet.Tests.Server.Unit;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Testcontainers.MySql;
using YDotNet.Server.EntityFramework;
using YDotNet.Server.Storage;

[TestFixture]
[Category("Docker")]
public class EFDocumentStorageMySqlFixture : DocumentStorageTests
{
    private readonly MySqlContainer mysql = new MySqlBuilder().Build();
    private IServiceProvider services;

    public class AppDbContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseYDotNet();
            base.OnModelCreating(modelBuilder);
        }
    }

    [OneTimeSetUp]
    public async Task OneTimeSetup()
    {
        await mysql.StartAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTeardown()
    {
        await mysql.StopAsync();
    }

    [SetUp]
    public async Task Setup()
    {
        services = new ServiceCollection()
            .AddLogging()
            .AddDbContext<AppDbContext>(options =>
            {
                options.UseMySql(mysql.GetConnectionString(), ServerVersion.AutoDetect(mysql.GetConnectionString()));
            })
            .AddYDotNet()
            .AddEntityFrameworkStorage<AppDbContext>()
            .Services
            .BuildServiceProvider();

        foreach (var service in services.GetRequiredService<IEnumerable<IHostedService>>())
        {
            await service.StartAsync(default);
        }

        var factory = services.GetRequiredService<IDbContextFactory<AppDbContext>>();
        var context = await factory.CreateDbContextAsync();
        var creator = (RelationalDatabaseCreator)context.Database.GetService<IDatabaseCreator>();

        await creator.EnsureCreatedAsync();
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
