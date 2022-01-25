using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    public AvdHost? GetAvdHost(string id)
    {
        Task<AvdHost?> getFromDbTask = Task.Run(async () => await GetAvdHostAsync(id));

        return getFromDbTask.Result;
    }

    private async Task<AvdHost?> GetAvdHostAsync(string id)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        AvdHost? hostItem;

        try
        {
            hostItem = await container.ReadItemAsync<AvdHost>(
                id: id,
                partitionKey: new("avd-host-items")
            );
        }
        catch
        {
            hostItem = null;
        }

        return hostItem;
    }
}