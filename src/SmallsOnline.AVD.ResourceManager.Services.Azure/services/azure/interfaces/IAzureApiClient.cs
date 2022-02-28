namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public interface IAzureApiClient
{
    bool IsBeingRefreshed { get; }

    void RefreshAccessToken(bool forceRefresh);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage);
}