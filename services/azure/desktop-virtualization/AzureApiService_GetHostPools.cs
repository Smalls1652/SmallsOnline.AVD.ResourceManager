using Azure.ResourceManager.Resources;

using SmallsOnline.AVD.ResourceManager.Models.Azure.Generic;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    public List<HostPool>? GetHostPools()
    {
        Task<List<HostPool>?> apiCallTask = Task.Run(async () => await GetHostPoolsAsync());

        return apiCallTask.Result;
    }
    private async Task<List<HostPool>?> GetHostPoolsAsync()
    {
        Subscription defaultSubscription = await armClient.GetDefaultSubscriptionAsync();

        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"subscriptions/{defaultSubscription.Data.SubscriptionGuid}/providers/Microsoft.DesktopVirtualization/hostPools?api-version=2021-07-12"
        );

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

        return hostPools;
    }
}