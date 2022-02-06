using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{
    /// <summary>
    /// Get all of the registered Azure Virtual Desktop hostpools in the database.
    /// </summary>
    /// <returns>An array/collection of <see cref="AvdHostPool" /> objects.</returns>
    public List<AvdHostPool> GetAvdHostPools()
    {
        Task<List<AvdHostPool>> getAvdHostPoolsTask = Task.Run(async () => await GetAvdHostPoolsAsync());

        List<AvdHostPool> hostPoolItems;
        try
        {
            hostPoolItems = getAvdHostPoolsTask.Result;
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

        return hostPoolItems;
    }

    /// <summary>
    /// Get all of the registered Azure Virtual Desktop hostpools in the database.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetAvdHostPools()" /> method.
    /// </remarks>
    /// <returns>An array/collection of <see cref="AvdHostPool" /> objects.</returns>
    private async Task<List<AvdHostPool>> GetAvdHostPoolsAsync()
    {
        Container container = cosmosDbClient.GetContainer(AppSettings.GetSetting("CosmosDbDatabaseName"), "monitored-hosts");
        QueryDefinition queryDefinition = new("SELECT * FROM c WHERE c.partitionKey = \"avd-hostpool-items\"");

        List<AvdHostPool> hostPoolItems = new();
        try
        {
            FeedIterator<AvdHostPool> queryIterator = container.GetItemQueryIterator<AvdHostPool>(queryDefinition);
            while (queryIterator.HasMoreResults)
            {
                foreach (AvdHostPool item in await queryIterator.ReadNextAsync())
                {
                    hostPoolItems.Add(item);
                }
            }

            queryIterator.Dispose();
        }
        catch (CosmosException errorDetails)
        {
            throw errorDetails;
        }


        return hostPoolItems;
    }
}