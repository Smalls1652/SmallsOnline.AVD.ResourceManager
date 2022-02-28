using System.Net;
using System.Web;
using Microsoft.AspNetCore.Mvc;

using SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.Generic;
using SmallsOnline.AVD.ResourceManager.Lib.Models.Azure.DesktopVirtualization;
using SmallsOnline.AVD.ResourceManager.Services.Azure;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure.Controllers;

[Route("api/virtualDesktop/hostPool")]
[ApiController]
public class HostPoolController : ControllerBase
{
    private readonly IAzureApiService _azureApiClient;
    private readonly ILogger _logger;

    public HostPoolController(IAzureApiService azureApiService, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<HostPoolController>();
        _azureApiClient = azureApiService;
    }

    [HttpGet("{hostPoolName?}")]
    public async Task<List<HostPool>?> GetHostPool(string? hostPoolName)
    {
        List<HostPool>? hostPools = await _azureApiClient.GetHostPoolsAsync();

        List<HostPool>? foundHostPoolItems = null;
        if (hostPools is not null && hostPoolName is not null)
        {
            string decodedHostPoolName = HttpUtility.UrlDecode(hostPoolName);
            foundHostPoolItems = hostPools.FindAll(
                (HostPool item) => item.Name == decodedHostPoolName
            );
        }
        else
        {
            foundHostPoolItems = hostPools;
        }

        return foundHostPoolItems;
    }
}