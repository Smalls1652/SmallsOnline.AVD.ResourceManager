using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;

using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;
using SmallsOnline.AVD.ResourceManager.Models.Json;

namespace SmallsOnline.AVD.ResourceManager.Functions;

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

        List<AvdHostPool> retrievedHostPools = _cosmosDbService.GetAvdHostPools();

        foreach (AvdHostPool hostPoolItem in retrievedHostPools)
        {
            _logger.LogInformation("Getting session hosts for hostpool: {HostPoolName}", hostPoolItem.HostPoolName);

            List<SessionHost>? sessionHosts = _azureApiService.GetSessionHosts(hostPoolItem);
            if (sessionHosts is not null)
            {
                foreach (SessionHost sessionHostItem in sessionHosts)
                {
                    _logger.LogInformation("{Name} - Sessions Active: {Sessions}", sessionHostItem.Name, sessionHostItem.Properties?.Sessions);

                    VirtualMachine virtualMachine = _azureApiService.GetAzVM(sessionHostItem.Properties?.ResourceId);
                    VirtualMachineData virtualMachineData = virtualMachine.Get(
                        expand: InstanceViewTypes.InstanceView
                    )
                        .Value
                        .Data;

                    AvdHost? avdHostData;
                    avdHostData = _cosmosDbService.GetAvdHost(sessionHostItem.Properties?.ObjectId);

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

                    if (sessionHostItem.Properties?.Sessions == 0 && sessionHostItem.Properties?.Status == "Available" && sessionHostItem.Properties.AllowNewSession == true)
                    {
                        if (sessionHostItem.Properties?.ResourceId is not null)
                        {
                            avdHostData.IncrementNoSessionsCount();
                            bool? shouldShutdown = avdHostData.ShouldShutdown(
                                threshold: 2
                            );

                            _logger.LogWarning("{Name} - No active session count incremented.", sessionHostItem.Name);

                            if (shouldShutdown == true)
                            {
                                _logger.LogWarning("{Name} - No active sessions for the last 2 evals. Deallocating session host.", sessionHostItem.Name);
                                virtualMachine.Deallocate(
                                    waitForCompletion: false
                                );

                                avdHostData.ResetNoSessionsCount();
                            }

                        }
                    }
                    else
                    {
                        _logger.LogInformation("{Name} - Session host either offline or has an active session.", sessionHostItem.Name);
                        avdHostData.ResetNoSessionsCount();
                    }

                    _cosmosDbService.UpdateAvdHost(avdHostData);
                }
            }
        }
    }
}
