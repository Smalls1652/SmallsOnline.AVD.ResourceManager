using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// Create a new instance of an <see cref="ArmClient" />. 
    /// </summary>
    /// <param name="azureCredential">The credential for authenticating the client.</param>
    /// <returns>An <see cref="ArmClient" /> instance.</returns>
    private static ArmClient CreateArmClient(DefaultAzureCredential azureCredential)
    {
        return new(
            credential: azureCredential
        );
    }
}