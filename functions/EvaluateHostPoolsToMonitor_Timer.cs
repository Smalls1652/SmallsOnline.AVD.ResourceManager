using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;
using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Functions;

/// <summary>
/// A timer function that executes every 30 minutes to find hostpools that the managed identity has permissions to access and add them to the database, if they're not there already.
/// </summary>
public class EvaluateHostPoolsToMonitor
{
    private readonly ILogger _logger;
    private readonly IAzureApiService _azureApiService;
    private readonly ICosmosDbService _cosmosDbService;

    public EvaluateHostPoolsToMonitor(ILoggerFactory loggerFactory, IAzureApiService azureApiService, ICosmosDbService cosmosDbService)
    {
        _logger = loggerFactory.CreateLogger<EvaluateHostPoolsToMonitor>();
        _azureApiService = azureApiService;
        _cosmosDbService = cosmosDbService;
    }

    [Function("EvaluateHostPoolsToMonitor_Timer")]
    public void Run(
        [TimerTrigger(
            schedule: "0 */30 * * * *",
            RunOnStartup = false,
            UseMonitor = true
        )]
        TimerInfo timerData
    )
    {
        _logger.LogInformation("Timer initiated for finding new hostpools to monitor.");

        // Get the hostpools that the managed identity has access to.
        // Then get the hostpools registered in the database already.
        _logger.LogInformation("Getting hostpools in subscription.");
        List<HostPool>? hostPools = _azureApiService.GetHostPools();

        _logger.LogInformation("Getting hostpools already configured for monitoring in DB.");
        List<AvdHostPool> hostPoolsInDb = _cosmosDbService.GetAvdHostPools();

        // If the Azure API response is not null, loop through each found hostpool and add them to the DB, if not already configured.
        List<HostPool> addedHostPools = new();
        if (hostPools is not null)
        {
            foreach (HostPool hostPoolItem in hostPools)
            {
                // Try to find the hostpool entry in the DB.
                AvdHostPool? foundDbItem = hostPoolsInDb.Find(
                    (AvdHostPool item) => item.HostPoolResourceId == hostPoolItem.Id
                );

                // If the hostpool entry is not found, then add it to the DB.
                if (foundDbItem is null)
                {
                    _logger.LogInformation("Adding '{Id}' to monitored hostpools.", hostPoolItem.Id);

                    addedHostPools.Add(hostPoolItem);
                    _cosmosDbService.AddHostPool(hostPoolItem);
                }
            }
        }

        // If no new hostpools were added, then log that nothing was added.
        if (addedHostPools.Count == 0)
        {
            _logger.LogInformation("No new hostpools were added for monitoring.");
        }
    }
}