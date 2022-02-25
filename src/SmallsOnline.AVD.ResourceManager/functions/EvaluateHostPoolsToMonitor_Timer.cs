using SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;
using SmallsOnline.AVD.ResourceManager.Lib.Models.Database;

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
        List<HostPoolDbEntry> hostPoolsInDb = _cosmosDbService.GetAvdHostPools();

        // If the Azure API response is not null, loop through each found hostpool and add them to the DB, if not already configured.
        List<HostPool> addedHostPools = new();
        List<HostPoolDbEntry> removedHostPools = new();

        _logger.LogInformation("Checking for any new hostpools to add.");
        if (hostPools is not null)
        {
            // Check to see if the hostpool pulled from the Azure API is in the DB.
            // If it isn't, then add it.
            foreach (HostPool hostPoolItem in hostPools)
            {
                // Try to find the hostpool entry in the DB.
                HostPoolDbEntry? foundDbItem = hostPoolsInDb.Find(
                    (HostPoolDbEntry item) => item.HostPoolResourceId == hostPoolItem.Id
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
        // Otherwise log how many were added.
        if (addedHostPools.Count == 0)
        {
            _logger.LogInformation("No new hostpools were added for monitoring.");
        }
        else
        {
            _logger.LogInformation("{Count} hostpools were added for monitoring.", addedHostPools.Count);
        }

        _logger.LogInformation("Checking for any hostpools to remove.");
        // If the DB response is not null, loop through each found hostpool and remove them from the DB if they aren't accessible anymore.
        if (hostPoolsInDb is not null && hostPools is not null)
        {
            // Check to see if the hostpool pulled from the DB was also found in the Azure API.
            // If it isn't, then remove it.
            foreach (HostPoolDbEntry hostPoolDbItem in hostPoolsInDb)
            {
                // Try to find the hostpool entry from what was pulled from Azure.
                HostPool? foundHostPoolItem = hostPools.Find(
                    (HostPool item) => item.Id == hostPoolDbItem.HostPoolResourceId
                );

                // If the hostpool entry is not found, then remove it from the DB.
                if (foundHostPoolItem is null)
                {
                    _logger.LogInformation("Removing '{HostPoolResourceId}' from the database, since it's no longer accessible.", hostPoolDbItem.HostPoolResourceId);
                    removedHostPools.Add(hostPoolDbItem);
                    _cosmosDbService.RemoveHostPool(hostPoolDbItem);

                    // Start the clean up process of session hosts from the hostpool that were registered in the DB.
                    _logger.LogInformation("Removing any session host from the DB that was registered to {HostPoolResourceId}", hostPoolDbItem.HostPoolResourceId);
                    List<SessionHostDbEntry>? hostsInDb = _cosmosDbService.GetAvdHosts(hostPoolDbItem.HostPoolResourceId);

                    // If hostsInDb is not null, then remove each host from the DB.
                    if (hostsInDb is not null)
                    {
                        foreach (SessionHostDbEntry hostItem in hostsInDb)
                        {
                            _cosmosDbService.RemoveAvdHost(hostItem);
                            _logger.LogInformation("'{VmResourceId}' was removed from the DB.", hostItem.VmResourceId);
                        }
                    }
                }
            }
        }

        // If no hostpools were removed, then log that nothing was removed.
        // Otherwise log how many were removed.
        if (removedHostPools.Count == 0)
        {
            _logger.LogInformation("No hostpools were removed from monitoring.");
        }
        else
        {
            _logger.LogInformation("{Count} hostpools were removed from monitoring.", removedHostPools.Count);
        }
    }
}