using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.Database;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Update an Azure Virtual Desktop session host in the database.
    /// </summary>
    /// <param name="hostItem">The updated data of the item.</param>
    public void UpdateAvdHost(SessionHostDbEntry hostItem)
    {
        Task getFromDbTask = Task.Run(async () => await UpdateAvdHostAsync(hostItem));

        getFromDbTask.Wait();
    }

    /// <summary>
    /// Update an Azure Virtual Desktop session host in the database.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="UpdateAvdHost()" /> method.
    /// </remarks>
    /// <param name="hostItem">The updated data of the item.</param>
    private async Task UpdateAvdHostAsync(SessionHostDbEntry hostItem)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        try
        {
            await container.UpsertItemAsync(
                item: hostItem,
                partitionKey: new("avd-host-items")
            );
        }
        catch (CosmosException errorDetails)
        {
            switch (errorDetails.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    await container.CreateItemAsync(
                        item: hostItem,
                        partitionKey: new("avd-host-items")
                    );
                    break;

                default:
                    throw errorDetails;
            }
        }
    }
}