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
    public AzureApiService()
    {
        armClient = new(
            credential: defaultAzureCred
        );

        apiClient = CreateApiClient(defaultAzureCred);
    }

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
    private ArmClient armClient;

    /// <summary>
    /// A generic Azure API client for accessing API endpoints not available in the Azure .NET SDK.
    /// </summary>
    private HttpClient apiClient;

    /// <summary>
    /// Create the <see cref="armClient /> and <see cref="apiClient" /> objects to be used with the service.
    /// </summary>
    /// <param name="azureCredential">The credential used for authenticating to Azure.</param>
    /// <returns></returns>
    private static HttpClient CreateApiClient(DefaultAzureCredential azureCredential)
    {
        HttpClient client = new();

        AccessToken accessToken = Task.Run(async () => await GetAccessTokenAsync(azureCredential)).Result;
        client.DefaultRequestHeaders.Add(
            name: "Authorization",
            value: $"Bearer {accessToken.Token}"
        );

        client.BaseAddress = new("https://management.azure.com/");

        return client;
    }

    /// <summary>
    /// Get the access token for authorizing API calls.
    /// </summary>
    /// <param name="azureCredential">The credential used for authenticating to Azure.</param>
    /// <returns></returns>
    private static async Task<AccessToken> GetAccessTokenAsync(DefaultAzureCredential azureCredential)
    {
        AccessToken accessToken = await azureCredential.GetTokenAsync(
            requestContext: new(
                scopes: new[] { "https://management.azure.com/.default" }
            )
        );

        return accessToken;
    }
}