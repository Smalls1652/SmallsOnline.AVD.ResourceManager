using Azure.Core;
using Azure.Identity;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public class AzureApiClient : IAzureApiClient, IDisposable
{
    private readonly DefaultAzureCredential _defaultAzureCredential;
    private readonly ILogger _logger;
    private bool _isDisposed = false;

    public AzureApiClient(DefaultAzureCredential defaultAzureCredential, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AzureApiClient>();
        _defaultAzureCredential = defaultAzureCredential;
        GetAccessToken();
        _apiClient = CreateApiClient(_accessToken);

        _logger.LogInformation("Initialized custom Azure API client.");
    }

    public bool IsBeingRefreshed
    {
        get => _isBeingRefreshed;
    }
    private bool _isBeingRefreshed = false;

    private HttpClient _apiClient;
    private AccessToken _accessToken;
    private DateTimeOffset _accessTokenAcquiredDateTime;

    public void RefreshAccessToken(bool forceRefresh)
    {
        _logger.LogInformation("Starting refresh access token check.");

        _isBeingRefreshed = true;

        DateTimeOffset tokenRefreshTime = _accessTokenAcquiredDateTime.AddMinutes(10);

        int compareResult = DateTimeOffset.Compare(DateTimeOffset.Now, tokenRefreshTime);

        bool shouldRefresh = compareResult switch
        {
            -1 => false,
            _ => true
        };

        if (shouldRefresh || forceRefresh)
        {
            _logger.LogInformation("Access token will be refreshed.");
            GetAccessToken();

            _apiClient.Dispose();
            _apiClient = CreateApiClient(_accessToken);

            _logger.LogInformation("Access token has been refreshed.");
        }

        _isBeingRefreshed = false;

        _logger.LogInformation("Access token refresh check completed.");
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
    {
        if (_isBeingRefreshed)
        {
            _logger.LogInformation("Token is currently being refreshed, waiting for it to complete.");
            while (_isBeingRefreshed)
            {
                await Task.Delay(500);
            }
        }
        else
        {
            RefreshAccessToken(
                forceRefresh: false
            );
        }

        HttpResponseMessage responseMessage = await _apiClient.SendAsync(requestMessage);

        return responseMessage;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                _apiClient.Dispose();
            }

            _isDisposed = true;
        }
    }

    private static HttpClient CreateApiClient(AccessToken accessToken)
    {
        HttpClient client = new();

        client.DefaultRequestHeaders.Add(
            name: "Authorization",
            value: $"Bearer {accessToken.Token}"
        );

        client.BaseAddress = new("https://management.azure.com/");

        return client;
    }

    private void GetAccessToken()
    {
        Task getAccessTokenTask = Task.Run(async () => await GetAccessTokenAsync());

        getAccessTokenTask.Wait();
    }

    private async Task GetAccessTokenAsync()
    {
        AccessToken accessToken = await _defaultAzureCredential.GetTokenAsync(
            requestContext: new(
                scopes: new[] { "https://management.azure.com/.default" }
            )
        );

        _accessTokenAcquiredDateTime = DateTimeOffset.Now;
        _accessToken = accessToken;
    }
}