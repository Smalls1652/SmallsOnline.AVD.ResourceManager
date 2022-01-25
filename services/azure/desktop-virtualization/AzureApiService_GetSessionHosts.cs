using System.Web;

using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.Generic;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;
namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    public List<SessionHost>? GetSessionHosts(AvdHostPool hostPool)
    {
        Task<List<SessionHost>?> apiCallTask = Task.Run(async () => await GetSessionHostsAsync(hostPool));

        return apiCallTask.Result;
    }

    private async Task<List<SessionHost>?> GetSessionHostsAsync(AvdHostPool hostPool)
    {
        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"{hostPool.HostPoolResourceId}/sessionHosts?api-version=2021-07-12"
        );

        HttpResponseMessage responseMessage = await apiClient.SendAsync(requestMessage);

        List<SessionHost>? sessionHosts = null;
        if (responseMessage.Content is not null)
        {
            string responseBody = await responseMessage.Content.ReadAsStringAsync();
            AzureResponseCollection<SessionHost>? rspCollection = JsonSerializer.Deserialize<AzureResponseCollection<SessionHost>?>(responseBody);
            sessionHosts = rspCollection?.Value;
        }

        responseMessage.Dispose();
        requestMessage.Dispose();

        return sessionHosts;
    }
}