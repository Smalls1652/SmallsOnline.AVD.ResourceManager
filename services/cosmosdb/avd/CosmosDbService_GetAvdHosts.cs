using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    public List<AvdHost>? GetAvdHosts()
    {
        Task<List<AvdHost>?> getFromDbTask = Task.Run(async () => await GetAvdHostsAsync());

        return getFromDbTask.Result;
    }

    private async Task<List<AvdHost>?> GetAvdHostsAsync()
    {
        List<AvdHost>? retrievedAvdHosts = new();

        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");
        QueryDefinition queryDef = new($"SELECT * FROM c WHERE c.partitionKey = \"avd-host-items\"");

        FeedIterator<AvdHost> containerQueryIterator = container.GetItemQueryIterator<AvdHost>(queryDef);
        while (containerQueryIterator.HasMoreResults)
        {
            foreach (AvdHost hostItem in await containerQueryIterator.ReadNextAsync())
            {
                retrievedAvdHosts.Add(hostItem);
            }
        }

        containerQueryIterator.Dispose();

        return retrievedAvdHosts;
    }
}