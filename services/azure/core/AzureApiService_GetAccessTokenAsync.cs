using Azure.Core;
using Azure.Identity;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
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