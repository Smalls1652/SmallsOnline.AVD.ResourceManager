using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

public partial class CosmosDbService : ICosmosDbService
{

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