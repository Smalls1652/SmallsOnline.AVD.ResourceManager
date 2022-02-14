using Azure.ResourceManager.Resources;

using SmallsOnline.AVD.ResourceManager.Models.Azure.Generic;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

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
        // Refresh the API client with a new token.
        UpdateApiClient();
        
        Subscription defaultSubscription = await armClient.GetDefaultSubscriptionAsync();

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"subscriptions/{defaultSubscription.Data.SubscriptionGuid}/providers/Microsoft.DesktopVirtualization/hostPools?api-version=2021-07-12"
        );

        logger.LogInformation("Sending API call to '{RequestUri}'", requestMessage.RequestUri);
        HttpResponseMessage responseMessage = await apiClient.SendAsync(requestMessage);

        List<HostPool>? hostPools = null;
        if (responseMessage.Content is not null)
        {
            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            AzureResponseCollection<HostPool>? rspCollection = JsonSerializer.Deserialize<AzureResponseCollection<HostPool>?>(responseBody);
            hostPools = rspCollection?.Value;
        }

        responseMessage.Dispose();
        requestMessage.Dispose();

        List<HostPool>? personalHostPools = null;
        if (hostPools is not null)
        {
            personalHostPools = hostPools.FindAll(
                (HostPool item) => item.Properties.HostPoolType == "Personal"
            );
        }

        return personalHostPools;
    }
}