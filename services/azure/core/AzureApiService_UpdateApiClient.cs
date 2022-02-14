namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// Set the <see cref="apiClient" /> with a new instance.
    /// </summary>
    private void UpdateApiClient()
    {
        if (apiClient is not null)
        {
            logger.LogInformation("Updating the Azure ApiClient with a new access token.");
            apiClient.Dispose();
        }
        else
        {
            logger.LogInformation("Creating the Azure ApiClient.");
        }

        apiClient = CreateApiClient(defaultAzureCred);
    }
}