using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get data about an Azure Virtual Desktop host in the database.
    /// </summary>
    /// <param name="id">The ID of the host.</param>
    /// <returns>An <see cref="AvdHost" /> object.</returns>
    public AvdHost? GetAvdHost(string id)
    {
        Task<AvdHost?> getFromDbTask = Task.Run(async () => await GetAvdHostAsync(id));

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