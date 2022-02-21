using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.Database;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get an Azure Virtual Desktop HostPool from the database.
    /// </summary>
    /// <param name="id">The unique ID of the hostpool in the database.</param>
    /// <returns>An <see cref="AvdHostPool" /> object.</returns>
    public HostPoolDbEntry GetAvdHostPool(string id)
    {
        Task<HostPoolDbEntry> getAvdHostPoolTask = Task.Run(async () => await GetAvdHostPoolAsync(id));

        HostPoolDbEntry hostPoolItem;
        try
        {
            hostPoolItem = getAvdHostPoolTask.Result;
        }
        catch (AggregateException errorDetails)
        {
            if (errorDetails.InnerException is not null)
            {
                throw errorDetails.InnerException;
            }
            else
            {
                throw errorDetails;
            }
        }

        return hostPoolItem;
    }

    /// <summary>
    /// Get an Azure Virtual Desktop HostPool from the database.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetAvdHostPool()" /> method.
    /// </remarks>
    /// <param name="id">The unique ID of the hostpool in the database.</param>
    /// <returns>An <see cref="AvdHostPool" /> object.</returns>
    private async Task<HostPoolDbEntry> GetAvdHostPoolAsync(string id)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        ItemResponse<HostPoolDbEntry> hostPoolItem;
        try
        {
            hostPoolItem = await container.ReadItemAsync<HostPoolDbEntry>(
                id: id,
                partitionKey: new("avd-hostpool-items")
            );
        }
        catch (CosmosException errorDetails)
        {
            throw errorDetails;
        }


        return hostPoolItem.Resource;
    }
}