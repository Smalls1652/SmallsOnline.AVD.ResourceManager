using Azure.ResourceManager;
using Azure.ResourceManager.Compute;

using SmallsOnline.AVD.ResourceManager.Models.AVD;

namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public partial class AzureApiService : IAzureApiService
{
    /// <summary>
    /// Get details about a <see cref="VirtualMachine" /> in Azure.
    /// </summary>
    /// <param name="resourceId">The resource ID of the virtual machine.</param>
    /// <returns></returns>
    public VirtualMachine GetAzVM(string resourceId)
    {
        VirtualMachine vmItem = armClient.GetVirtualMachine(
            id: new(resourceId)
        );

        return vmItem;
    }
}