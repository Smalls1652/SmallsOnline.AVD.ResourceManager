using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.Database;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get all of the registered Azure Virtual Desktop session hosts in the database.
    /// </summary>
    /// <returns>An array/collection of <see cref="AvdHost" /> hosts.</returns>
    public List<SessionHostDbEntry>? GetAvdHosts(string? hostPoolResourceId = null)
    {
        Task<List<SessionHostDbEntry>?> getFromDbTask = Task.Run(async () => await GetAvdHostsAsync(hostPoolResourceId));

        return getFromDbTask.Result;
    }

    /// <summary>
    /// Get all of the registered Azure Virtual Desktop session hosts in the database.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetAvdHosts()" /> method.
    /// </remarks>
    /// <returns>An array/collection of <see cref="AvdHost" /> hosts.</returns>
    private async Task<List<SessionHostDbEntry>?> GetAvdHostsAsync(string? hostPoolResourceId = null)
    {
        List<SessionHostDbEntry>? retrievedAvdHosts = new();

        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        // Create a query based off whether or not the hostpool resource ID was supplied.
        QueryDefinition queryDef = string.IsNullOrEmpty(hostPoolResourceId) switch
        {
            false => new($"SELECT * FROM c WHERE c.partitionKey = \"avd-host-items\" and c.hostPoolResourceId = \"{hostPoolResourceId}\""),
            _ => new($"SELECT * FROM c WHERE c.partitionKey = \"avd-host-items\"")
        };

        FeedIterator<SessionHostDbEntry> containerQueryIterator = container.GetItemQueryIterator<SessionHostDbEntry>(queryDef);
        while (containerQueryIterator.HasMoreResults)
        {
            foreach (SessionHostDbEntry hostItem in await containerQueryIterator.ReadNextAsync())
            {
                retrievedAvdHosts.Add(hostItem);
            }
        }

        containerQueryIterator.Dispose();

        return retrievedAvdHosts;
    }
}