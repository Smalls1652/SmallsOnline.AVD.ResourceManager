using Azure.Core;
using Azure.Identity;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

/// <summary>
/// A custom API client for communicating with the Azure API.
/// </summary>
public class AzureApiClient : IAzureApiClient, IDisposable
{
    /// <summary>
    /// The credential being used to authenticate to Azure.
    /// </summary>
    private readonly DefaultAzureCredential _defaultAzureCredential;

    /// <summary>
    /// The logger for writing log messages.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// The current disposed status.
    /// </summary>
    private bool _isDisposed = false;

    public AzureApiClient(DefaultAzureCredential defaultAzureCredential, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<AzureApiClient>();
        _defaultAzureCredential = defaultAzureCredential;
        GetAccessToken();
        _apiClient = CreateApiClient(_accessToken);

        _logger.LogInformation("Initialized custom Azure API client.");
    }

    /// <summary>
    /// Whether the token is currently being refreshed on the API client.
    /// </summary>
    public bool IsBeingRefreshed
    {
        get => _isBeingRefreshed;
    }
    private bool _isBeingRefreshed = false;

    /// <summary>
    /// The <see cref="HttpClient" /> instance that sends the API calls.
    /// </summary>
    private HttpClient _apiClient;

    /// <summary>
    /// The current <see cref="AccessToken" /> for authorizing API calls.
    /// </summary>
    private AccessToken _accessToken;

    /// <summary>
    /// When the <see cref="_accessToken" /> was last acquired.
    /// </summary>
    private DateTimeOffset _accessTokenAcquiredDateTime;

    /// <summary>
    /// Refresh the <see cref="AccessToken" /> used for authorizing API calls to the Azure API.
    /// </summary>
    /// <param name="forceRefresh">Force the <see cref="AccessToken" /> to refresh.</param>
    public void RefreshAccessToken(bool forceRefresh = false)
    {
        _logger.LogInformation("Starting refresh access token check.");

        // Set that the token is currently being refreshed.
        _isBeingRefreshed = true;

        // Instead of basing off the expiration time defined in the 'ExpiresOn' property for the AccessToken object,
        // we're invalidating it 10-minutes after execution. This is being done to ensure that the identity is able to
        // see newly assigned or provisioned resources.
        DateTimeOffset tokenRefreshTime = _accessTokenAcquiredDateTime.AddMinutes(10);
        int compareResult = DateTimeOffset.Compare(DateTimeOffset.Now, tokenRefreshTime);
        bool shouldRefresh = compareResult switch
        {
            -1 => false,
            _ => true
        };

        // If shouldRefresh or forceRefresh are set to true, start the access token refresh.
        if (shouldRefresh || forceRefresh)
        {
            _logger.LogInformation("Access token will be refreshed.");
            // Get a new access token.
            GetAccessToken();

            // Dispose the old HttpClient and create a new one with the newly created access token.
            _apiClient.Dispose();
            _apiClient = CreateApiClient(_accessToken);

            _logger.LogInformation("Access token has been refreshed.");
        }

        // Set that the token is done being refreshed.
        _isBeingRefreshed = false;
        _logger.LogInformation("Access token refresh check completed.");
    }

    /// <summary>
    /// Send an HTTP request as an asynchronous operation.
    /// </summary>
    /// <remarks>
    /// This method is a wrapper for the <see cref="HttpClient" />'s SendAsync() method, with the added benefit of waiting for the token to be refreshed.
    /// </remarks>
    /// <param name="requestMessage">An <see cref="HttpRequestMessage" /> item.</param>
    /// <returns>A <see cref="HttpResponseMessage" /> returned by the <see cref="HttpClient" />.</returns>
    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
    {
        // If the access token is already being refreshed, then wait for it to finish.
        // Otherwise, start the refresh process.
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

        // Send the request.
        HttpResponseMessage responseMessage = await _apiClient.SendAsync(requestMessage);

        return responseMessage;
    }

    /// <summary>
    /// Dispose the <see cref="AzureApiClient" />.
    /// </summary>
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

    /// <summary>
    /// Create a <see cref="HttpClient" /> for sending API calls to the Azure API.
    /// </summary>
    /// <param name="accessToken">An <see cref="AccessToken" /> for authorizing API calls.</param>
    /// <returns>A <see cref="HttpClient" /> for communicating with the Azure API.</returns>
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

    /// <inheritdoc cref="GetAccessTokenAsync()" />
    private void GetAccessToken()
    {
        Task getAccessTokenTask = Task.Run(async () => await GetAccessTokenAsync());

        getAccessTokenTask.Wait();
    }

    /// <summary>
    /// Gets an <see cref="AccessToken" /> and sets it to <see cref="_accessToken" />.
    /// </summary>
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