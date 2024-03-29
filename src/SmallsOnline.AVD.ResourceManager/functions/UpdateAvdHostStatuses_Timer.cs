using Azure;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;
using SmallsOnline.AVD.ResourceManager.Lib.Models.Database;

namespace SmallsOnline.AVD.ResourceManager.Functions;

/// <summary>
/// A timer function that executes every 20 minutes to evaluate the current status of session hosts and if they should shutdown/deallocate.
/// </summary>
public class UpdateAvdHostStatuses_Timer
{
    private readonly ILogger _logger;
    private readonly ICosmosDbService _cosmosDbService;
    private readonly IAzureApiService _azureApiService;

    public UpdateAvdHostStatuses_Timer(ILoggerFactory loggerFactory, ICosmosDbService cosmosDbService, IAzureApiService azureApiService)
    {
        _logger = loggerFactory.CreateLogger<UpdateAvdHostStatuses_Timer>();
        _cosmosDbService = cosmosDbService;
        _azureApiService = azureApiService;
    }

    [Function("UpdateAvdHostStatuses_Timer")]
    public void Run(
        [TimerTrigger(
            schedule: "0 */20 * * * *",
            RunOnStartup = false,
            UseMonitor = true
        )]
        TimerInfo timer
    )
    {
        _logger.LogInformation("Timer for updating sessions host was triggered.");

        // Get all of the hostpools registered in the database.
        List<HostPoolDbEntry> retrievedHostPools = _cosmosDbService.GetAvdHostPools();

        // Loop through each hostpool.
        foreach (HostPoolDbEntry hostPoolItem in retrievedHostPools)
        {
            _logger.LogInformation("Getting session hosts for hostpool: {HostPoolName}", hostPoolItem.HostPoolResourceId);

            // Get the session hosts in the hostpool.
            List<SessionHost>? sessionHosts = _azureApiService.GetSessionHosts(hostPoolItem);

            if (sessionHosts is not null)
            {
                // Start a check for session hosts that need to be removed from the DB.
                List<SessionHostDbEntry>? hostsInDb = _cosmosDbService.GetAvdHosts(hostPoolItem.HostPoolResourceId);
                if (hostsInDb is not null)
                {
                    foreach (SessionHostDbEntry hostItem in hostsInDb)
                    {
                        // See if the session host was returned by the Azure API.
                        SessionHost? foundSessionHost = sessionHosts.Find(
                            (SessionHost item) => item.Properties.ResourceId == hostItem.VmResourceId
                        );

                        // If it wasn't found, then remove it from the DB.
                        if (foundSessionHost is null)
                        {
                            _cosmosDbService.RemoveAvdHost(hostItem);
                            _logger.LogWarning("'{VmResourceId}' was removed from the DB.", hostItem.VmResourceId);
                        }
                    }
                }

                // Loop through each session host in the hostpool.
                foreach (SessionHost sessionHostItem in sessionHosts)
                {
                    // Need to add a better way to handle the 'Properties' property returning as null.
                    if (sessionHostItem.Properties is null)
                    {
                        _logger.LogError("{Name} - Session host's 'Properties' was returned as null.", sessionHostItem.Name);
                    }
                    else
                    {
                        _logger.LogInformation("{Name} - Sessions Active: {Sessions}", sessionHostItem.Name, sessionHostItem.Properties?.Sessions);

                        // Get details about the virtual machine.
                        // Note: The compiler is still returning a that the 'Properties' property is still possibily null, which it's definitely not.
                        // For the time being, I've added a 'null-forgiving' operator to it.
                        VirtualMachineResource virtualMachine = _azureApiService.GetAzVM(sessionHostItem.Properties!.ResourceId);
                        VirtualMachineData virtualMachineData = virtualMachine.Get(
                            expand: InstanceViewType.InstanceView
                        )
                            .Value
                            .Data;

                        // Get data about the session host in the database.
                        SessionHostDbEntry? avdHostData;
                        avdHostData = _cosmosDbService.GetAvdHost(sessionHostItem.Properties.ObjectId);

                        // If no data was returned from the database, then initialize an object for the session host.
                        if (avdHostData is null)
                        {
                            _logger.LogWarning("{Name} - An item for this session host doesn't exist in the DB. Creating one.", sessionHostItem.Name);
                            avdHostData = new(
                                virtualMachineData: virtualMachineData,
                                avdHostPool: hostPoolItem,
                                avdSessionHostData: sessionHostItem,
                                previousHostData: null
                            );
                        }

                        // If the session host has 0 active sessions, is available, and is not in drain mode (AllowNewSession is set to true),
                        // then start the evaluation process.
                        if (sessionHostItem.Properties?.Sessions == 0 && sessionHostItem.Properties?.Status == "Available" && sessionHostItem.Properties.AllowNewSession == true)
                        {
                            // If the session host's resource ID isn't null, then start the evaluation.
                            // This checks needs to be expanded on, since there is a slight possibility that the API might return a null value.
                            if (sessionHostItem.Properties?.ResourceId is not null)
                            {
                                // Increment the "NoSessionCount" property on the session host.
                                // Then evaluate if it needs to be shutdown.
                                // Currently the threshold is set to 2, so, if a session host has no active session for 40 minutes, it will return true.
                                // !! Need to add the ability to specify what the threshold should be.
                                avdHostData.IncrementNoSessionsCount();
                                bool? shouldShutdown = avdHostData.ShouldShutdown(
                                    threshold: 2
                                );

                                _logger.LogWarning("{Name} - No active session count incremented.", sessionHostItem.Name);

                                // If the session host was evaluated to shutdown, start the shutdown process.
                                if (shouldShutdown == true)
                                {
                                    _logger.LogWarning("{Name} - No active sessions for the last 2 evals. Deallocating session host.", sessionHostItem.Name);

                                    // Set the virtual machine for deallocation.
                                    // It's set to not wait for completion, so that the function doesn't hit the max timeout limit.
                                    virtualMachine.Deallocate(
                                        waitUntil: WaitUntil.Started
                                    );

                                    // Reset the "NoSessionCount" to 0, so that the it doesn't cause issues when it's evaluated in the future.
                                    avdHostData.ResetNoSessionsCount();
                                }

                            }
                        }
                        // If the session host is offline, in drain mode, or it has an active session, then reset the "NoSessionCount".
                        else
                        {
                            _logger.LogInformation("{Name} - Session host either offline or has an active session.", sessionHostItem.Name);
                            avdHostData.ResetNoSessionsCount();
                        }

                        // Update the entry for the session host in the database.
                        _cosmosDbService.UpdateAvdHost(avdHostData);
                    }
                }
            }
        }
    }
}
