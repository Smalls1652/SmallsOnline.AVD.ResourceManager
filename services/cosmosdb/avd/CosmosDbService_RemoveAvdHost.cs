using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Remove a hostpool, that is monitored by the resource manager, from the DB.
    /// </summary>
    /// <param name="hostItem">The session host to remove for monitoring.</param> 
    public void RemoveAvdHost(AvdHost hostItem)
    {
        Task removeHostTask = Task.Run(async () => await RemoveAvdHostAsync(hostItem));

        removeHostTask.Wait();
    }

    /// <inheritdoc cref="RemoveAvdHost()" />
    private async Task RemoveAvdHostAsync(AvdHost hostItem)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        await container.DeleteItemAsync<AvdHostPool>(
            id: hostItem.Id,
            partitionKey: new("avd-host-items")
        );
    }
}