using System.Text.Encodings.Web;
using Microsoft.Azure.Cosmos;

using SmallsOnline.AVD.ResourceManager.Helpers;
using SmallsOnline.AVD.ResourceManager.Models.Json;

namespace SmallsOnline.AVD.ResourceManager.Services.CosmosDb;

/// <summary>
/// An Azure CosmosDB client service. 
/// </summary>
public partial class CosmosDbService : ICosmosDbService
{
    public CosmosDbService(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<CosmosDbService>();

        logger.LogInformation("Initializing CosmosDbService.");
        cosmosDbClient = new(
            connectionString: AppSettings.GetSetting("CosmosDbConnectionString"),
            clientOptions: new()
            {
                Serializer = jsonSerializer
            }
        );
    }

    private CosmosClient cosmosDbClient;
    private readonly CosmosDbSerializer jsonSerializer = new(
        new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.Default,
            Converters = {
                new DateTimeOffsetConverter()
            }
        }
    );
    private readonly ILogger logger;
}