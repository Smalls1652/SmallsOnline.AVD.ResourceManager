using Azure.ResourceManager.Resources;
using SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;
using SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.Generic;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// Get the <see cref="HostPool" /> items in the Azure subscription.
    /// </summary>
    /// <returns>An array of <see cref="HostPool" /> items.</returns>
    public List<HostPool>? GetHostPools()
    {
        Task<List<HostPool>?> apiCallTask = Task.Run(async () => await GetHostPoolsAsync());

        return apiCallTask.Result;
    }

    /// <summary>
    /// Get the <see cref="HostPool" /> items in the Azure subscription.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetHostPools()" /> method.
    /// </remarks>
    /// <returns>An array of <see cref="HostPool" /> items.</returns>
    private async Task<List<HostPool>?> GetHostPoolsAsync()
    {
        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"subscriptions/{AppSettings.GetSetting("AzureSubscriptionId")!}/providers/Microsoft.DesktopVirtualization/hostPools?api-version=2021-07-12"
        );

        logger.LogInformation("Sending API call to '{RequestUri}'", requestMessage.RequestUri);
        HttpResponseMessage responseMessage = await azureApiClient.SendAsync(requestMessage);

        List<HostPool>? hostPools = null;
        if (responseMessage.Content is not null)
        {
            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            AzureResponseCollection<HostPool>? rspCollection = JsonSerializer.Deserialize<AzureResponseCollection<HostPool>?>(responseBody);
            hostPools = rspCollection?.Value;
        }

        responseMessage.Dispose();
        requestMessage.Dispose();

        List<HostPool>? hostPoolsToManage = null;
        if (hostPools is not null)
        {
            foreach (HostPool hostPoolItem in hostPools)
            {
                // Check to see if the hostpool passes all of these checks:
                // 1. The hostpool item's 'Tags' property is not null.
                // 2. Check to see if the 'Tags' property contains key with the name 'AVDResourceManagerEnabled'.
                // 3. The value for the 'AVDResourceManagerEnabled' key is "true".
                if (hostPoolItem.Tags is not null && hostPoolItem.Tags.ContainsKey("AVDResourceManagerEnabled") && hostPoolItem.Tags["AVDResourceManagerEnabled"] == "true")
                {
                    // If 'hostPoolsToManage' is null, initialize it.
                    if (hostPoolsToManage is null)
                    {
                        hostPoolsToManage = new();
                    }

                    // Add the hostpool item to 'hostPoolsToManage'.
                    hostPoolsToManage.Add(hostPoolItem);
                }
            }
        }

        return hostPoolsToManage;
    }
}