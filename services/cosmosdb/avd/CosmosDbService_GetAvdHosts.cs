using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get all of the registered Azure Virtual Desktop session hosts in the database.
    /// </summary>
    /// <returns>An array/collection of <see cref="AvdHost" /> hosts.</returns>
    public List<AvdHost>? GetAvdHosts(string? hostPoolResourceId = null)
    {
        Task<List<AvdHost>?> getFromDbTask = Task.Run(async () => await GetAvdHostsAsync(hostPoolResourceId));

        return getFromDbTask.Result;
    }

    /// <summary>
    /// Get all of the registered Azure Virtual Desktop session hosts in the database.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetAvdHosts()" /> method.
    /// </remarks>
    /// <returns>An array/collection of <see cref="AvdHost" /> hosts.</returns>
    private async Task<List<AvdHost>?> GetAvdHostsAsync(string? hostPoolResourceId = null)
    {
        List<AvdHost>? retrievedAvdHosts = new();

        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        // Create a query based off whether or not the hostpool resource ID was supplied.
        QueryDefinition queryDef = string.IsNullOrEmpty(hostPoolResourceId) switch
        {
            false => new($"SELECT * FROM c WHERE c.partitionKey = \"avd-host-items\" and c.hostPoolResourceId = \"{hostPoolResourceId}\""),
            _ => new($"SELECT * FROM c WHERE c.partitionKey = \"avd-host-items\"")
        };

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