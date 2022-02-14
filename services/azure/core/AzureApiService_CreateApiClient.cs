using Azure.Core;
using Azure.Identity;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
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
}