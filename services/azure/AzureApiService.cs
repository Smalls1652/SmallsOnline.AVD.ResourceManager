using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Core;
using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Helpers;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    public AzureApiService()
    {
        armClient = new(
            credential: defaultAzureCred
        );

        apiClient = CreateApiClient(defaultAzureCred);
    }

    private readonly DefaultAzureCredential defaultAzureCred = new(
        options: new()
        {
            ManagedIdentityClientId = AppSettings.GetSetting("ManagedIdentityClientId")
        }
    );
    private ArmClient armClient;
    private HttpClient apiClient;

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