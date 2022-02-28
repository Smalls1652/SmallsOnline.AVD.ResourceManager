using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder webBuilder = WebApplication.CreateBuilder();

        webBuilder.Services.AddControllers();
        webBuilder.Services.AddSingleton<IAzureApiService, AzureApiService>();
        webBuilder.Services.AddEndpointsApiExplorer();

        webBuilder.Logging.AddDebug();
        webBuilder.Logging.AddConsole();

        webBuilder.Host.ConfigureDefaults(args);

        WebApplication app = webBuilder.Build();

        app
            .UseRouting()
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseWebSockets()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(
            configure: (endpoints) =>
            {
                endpoints.MapControllers();
            }
        );

        app.Run();
    }
}