namespace Demo;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();
        builder.Services.AddYDotNet()
            .AddCallback<Listener>()
            .AddWebSockets();

        var app = builder.Build();

        app.UseStaticFiles();
        app.UseWebSockets();
        app.UseRouting();
        app.Map("/collaboration", builder =>
        {
            builder.UseYDotnetWebSockets();
        });

        app.Run();
    }
}
