using MongoDB.Driver;
using StackExchange.Redis;
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
                                builder.Configuration["Clustering:Redis:ConnectionString"]!);
                    });

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
