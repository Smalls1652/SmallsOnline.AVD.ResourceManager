using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.Database;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get data about an Azure Virtual Desktop host in the database.
    /// </summary>
    /// <param name="id">The ID of the host.</param>
    /// <returns>An <see cref="AvdHost" /> object.</returns>
    public SessionHostDbEntry? GetAvdHost(string id)
    {
        Task<SessionHostDbEntry?> getFromDbTask = Task.Run(async () => await GetAvdHostAsync(id));

        return getFromDbTask.Result;
    }

    /// <summary>
    /// Get data about an Azure Virtual Desktop host in the database.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetAvdHost()" /> method.
    /// </remarks>
    /// <param name="id">The ID of the host.</param>
    /// <returns>An <see cref="AvdHost" /> object.</returns>
    private async Task<SessionHostDbEntry?> GetAvdHostAsync(string id)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        SessionHostDbEntry? hostItem;

        try
        {
            hostItem = await container.ReadItemAsync<SessionHostDbEntry>(
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