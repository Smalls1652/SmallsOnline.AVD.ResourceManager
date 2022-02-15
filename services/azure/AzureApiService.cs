using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Core;
using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Helpers;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

/// <summary>
/// An API client for sending API calls to the Microsoft Azure service.
/// </summary>
public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// The default credential for the managed identity in Azure.
    /// </summary>
    private readonly DefaultAzureCredential defaultAzureCred = new(
        options: new()
        {
            ManagedIdentityClientId = AppSettings.GetSetting("ManagedIdentityClientId")
        }
    );

    /// <summary>
    /// The Azure Resource Manager API client.
    /// </summary>
    private readonly ArmClient armClient;

    /// <summary>
    /// A generic Azure API client for accessing API endpoints not available in the Azure .NET SDK.
    /// </summary>
    private readonly AzureApiClient azureApiClient;

    private readonly ILogger logger;

    public AzureApiService(ILoggerFactory loggerFactory)
    {
        logger = loggerFactory.CreateLogger<AzureApiService>();

        logger.LogInformation("Initializing AzureApiService.");
        armClient = CreateArmClient(defaultAzureCred);
        azureApiClient = new(
            defaultAzureCredential: defaultAzureCred,
            loggerFactory: loggerFactory
        );
    }
}