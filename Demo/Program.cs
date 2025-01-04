using Demo.Db;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using StackExchange.Redis;
using YDotNet.Server;
using YDotNet.Server.EntityFramework;
using YDotNet.Server.MongoDB;

namespace Demo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddRazorPages();

        var yDotNet =
            builder.Services.AddYDotNet()
                .AutoCleanup()
                .AddCallback<Callback>()
                .AddWebSockets();

        builder.Services.Configure<DocumentManagerOptions>(options =>
        {
            options.CacheDuration = TimeSpan.FromSeconds(10);
        });

        switch (builder.Configuration["Storage:Type"])
        {
            case "MongoDb":
                {
                    yDotNet.Services.AddSingleton<IMongoClient>(
                        _ => new MongoClient(builder.Configuration["Storage:MongoDb:ConnectionString"]));

                    yDotNet.AddMongoStorage(options =>
                    {
                        options.DatabaseName = builder.Configuration["Storage:MongoDb:DatabaseName"]!;
                    });

                    break;
                }

            case "Redis":
                {
                    yDotNet.AddRedisStorage();
                    yDotNet.AddRedis(options =>
                    {
                        options.Configuration =
                            ConfigurationOptions.Parse(
                                builder.Configuration["Storage:Redis:ConnectionString"]!);
                    });

                    break;
                }

            case "Postgres":
                {
                    builder.Services.AddHostedService<DbInitializer>();
                    builder.Services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseNpgsql(
                            builder.Configuration["Storage:Postgres:ConnectionString"]!);
                    });

                    yDotNet.AddEntityFrameworkStorage<AppDbContext>();

                    break;
                }
        }

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseWebSockets();
        app.UseRouting();

        app.MapControllers();
        app.Map("/collaboration", builder =>
        {
            builder.UseYDotnetWebSockets();
        });

        app.Run();
    }
}
