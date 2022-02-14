using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    public void AddHostPool(HostPool hostPool)
    {
        Task addHostPoolTask = Task.Run(async () => await AddHostPoolAsync(hostPool));

        addHostPoolTask.Wait();
    }

    private async Task AddHostPoolAsync(HostPool hostPool)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        AvdHostPool hostPoolDbItem = new()
        {
            Id = Guid.NewGuid().ToString(),
            PartitionKey = "avd-hostpool-items",
            HostPoolResourceId = hostPool.Id
        };

        try
        {
            await container.UpsertItemAsync(
                item: hostPoolDbItem,
                partitionKey: new("avd-hostpool-items")
            );
        }
        catch (CosmosException errorDetails)
        {
            switch (errorDetails.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    await container.CreateItemAsync(
                        item: hostPoolDbItem,
                        partitionKey: new("avd-hostpool-items")
                    );
                    break;

                default:
                    throw errorDetails;
            }
        }
    }
}