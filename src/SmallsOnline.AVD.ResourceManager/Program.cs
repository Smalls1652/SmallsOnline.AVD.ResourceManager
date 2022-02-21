using SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

namespace SmallsOnline.AVD.ResourceManager;

public class Program
{
    public static void Main()
    {
        IHost host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(
                (services) =>
                {
                    services.AddSingleton<ICosmosDbService, CosmosDbService>();
                    services.AddSingleton<IAzureApiService, AzureApiService>();
                }
            )
            .Build();

        host.Run();
    }
}
