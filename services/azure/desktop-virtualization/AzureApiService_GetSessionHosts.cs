using System.Web;

using SmallsOnline.AVD.ResourceManager.Models.AVD;
using SmallsOnline.AVD.ResourceManager.Models.Azure.Generic;
using SmallsOnline.AVD.ResourceManager.Models.Azure.DesktopVirtualization;
namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// Get the <see cref="SessionHost">SessionHosts</see> in an Azure Virtual Desktop hostpool.
    /// </summary>
    /// <param name="hostPool">The hostpool of the session hosts.</param>
    /// <returns>An array of all sessions hosts currently in the hostpool.</returns>
    public List<SessionHost>? GetSessionHosts(AvdHostPool hostPool)
    {
        Task<List<SessionHost>?> apiCallTask = Task.Run(async () => await GetSessionHostsAsync(hostPool));

        return apiCallTask.Result;
    }

    /// <summary>
    /// Get the <see cref="SessionHost">SessionHosts</see> in an Azure Virtual Desktop hostpool.
    /// </summary>
    /// <remarks>
    /// This method is for running the request asynchronously from the <see cref="GetSessionHosts()" /> method.
    /// </remarks>
    /// <param name="hostPool">The hostpool of the session hosts.</param>
    /// <returns>An array of all sessions hosts currently in the hostpool.</returns>
    private async Task<List<SessionHost>?> GetSessionHostsAsync(AvdHostPool hostPool)
    {
        HttpRequestMessage requestMessage = new(
            method: HttpMethod.Get,
            requestUri: $"{hostPool.HostPoolResourceId}/sessionHosts?api-version=2021-07-12"
        );

        logger.LogInformation("Sending API call to '{RequestUri}'", requestMessage.RequestUri);
        HttpResponseMessage responseMessage = await azureApiClient.SendAsync(requestMessage);

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