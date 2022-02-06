using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get an Azure Virtual Desktop HostPool from the database.
    /// </summary>
    /// <param name="id">The unique ID of the hostpool in the database.</param>
    /// <returns>An <see cref="AvdHostPool" /> object.</returns>
    public AvdHostPool GetAvdHostPool(string id)
    {
        Task<AvdHostPool> getAvdHostPoolTask = Task.Run(async () => await GetAvdHostPoolAsync(id));

        AvdHostPool hostPoolItem;
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
    private async Task<AvdHostPool> GetAvdHostPoolAsync(string id)
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");

        ItemResponse<AvdHostPool> hostPoolItem;
        try
        {
            hostPoolItem = await container.ReadItemAsync<AvdHostPool>(
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