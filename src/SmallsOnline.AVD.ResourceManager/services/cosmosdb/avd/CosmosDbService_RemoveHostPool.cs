using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.Database;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Remove a hostpool, that is monitored by the resource manager, from the DB.
    /// </summary>
    /// <param name="hostPoolItem">The hostpool to remove for monitoring.</param> 
    public void RemoveHostPool(HostPoolDbEntry hostPoolItem)
    {
        Task removeHostPoolTask = Task.Run(async () => await RemoveHostPoolAsync(hostPoolItem));

        removeHostPoolTask.Wait();
    }

    /// <summary>
    /// Remove a hostpool, that is monitored by the resource manager, from the DB.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="RemoveHostPool()" /> method.
    /// </remarks>
    /// <param name="hostItem">The hostpool to remove for monitoring.</param>
    private async Task RemoveHostPoolAsync(HostPoolDbEntry hostPoolItem)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        await container.DeleteItemAsync<HostPoolDbEntry>(
            id: hostPoolItem.Id,
            partitionKey: new("avd-hostpool-items")
        );
    }
}