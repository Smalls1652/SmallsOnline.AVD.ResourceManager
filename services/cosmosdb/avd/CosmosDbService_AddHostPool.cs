using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Add a hostpool, to be monitored by the resource manager, to the DB.
    /// </summary>
    /// <param name="hostPool">The hostpool to add for monitoring.</param>
    public void AddHostPool(HostPool hostPool)
    {
        Task addHostPoolTask = Task.Run(async () => await AddHostPoolAsync(hostPool));

        addHostPoolTask.Wait();
    }

    /// <summary>
    /// Add a hostpool, to be monitored by the resource manager, to the DB.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="AddHostPool()" /> method.
    /// </remarks>
    /// <param name="hostPool">The hostpool to add for monitoring.</param>
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