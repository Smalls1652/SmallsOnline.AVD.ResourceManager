using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    public void UpdateAvdHost(AvdHost hostItem)
    {
        Task getFromDbTask = Task.Run(async () => await UpdateAvdHostAsync(hostItem));

        getFromDbTask.Wait();
    }

    private async Task UpdateAvdHostAsync(AvdHost hostItem)
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